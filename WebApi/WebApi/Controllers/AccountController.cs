using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.Data;
using WebApi.Data.DTOs.AccountDtos.Roles;
using WebApi.Data.DTOs.AccountDtos;
using WebApi.Data.Models;
using WebApi.BLs.Interfaces;
using WebApi.BLs;
using WebApi.Exceptions;
using AutoMapper;
using WebApi.Data.DTOs;

namespace WebApi.Controllers
{
    [Controller]
    [Route("api/accounts")]
    public class AccountController : LoginedUserControllerBase
    {
        private readonly IJwtTokenBl _tokenBl;
        private readonly IUserBl _userBl;
        private readonly IAccountBl _accountBl;
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IConfiguration _configuration;
        private readonly INotificationBl _notificationBl;
        private readonly IMapper _mapper;

        public AccountController(
            IJwtTokenBl tokenBl,
            IUserBl userBl,
            IAccountBl accountBl,
            INotificationBl notificationBl,
            IMapper mapper
        )
        {
            _tokenBl = tokenBl;
            _userBl = userBl;
            _accountBl = accountBl;
            _notificationBl = notificationBl;
            _mapper = mapper;
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="newUserData">User name, email and password</param>
        /// <returns>Status of registration</returns>
        /// <response code="200">Register successes</response>
        /// <response code="401">Invalid user name, email and\or password</response>
        [HttpPost]
        [Route("register")]
        public async Task<object> Register([FromBody]RegisterDto newUserData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(e => e.Value.Errors.Select(e => e.ErrorMessage)));
            }

            try
            {
                if (!await _accountBl.CheckPassword(newUserData.Password))
                    return BadRequest("Password is not valid");
                await _accountBl.CreateAsync(newUserData);
                await _accountBl.AddToRoleAsync(await _accountBl.GetUserByName(newUserData.Name), "User");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        /// <summary>
        /// User log in
        /// </summary>
        /// <param name="userData">User name and password</param>
        /// <returns>User token</returns>
        /// <response code="200">Log in successes</response>
        /// <response code="401">Invalid user name and\or password</response>
        [HttpPost]
        [Route("login")]
        public async Task<object> Login([FromBody] LoginDto userData)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.SelectMany(e => e.Value.Errors.Select(e => e.ErrorMessage)));
            }

            User user = null;
            bool userNotFound = false;
            try
            {
                user = await _accountBl.GetUserByName(userData.Name);
            }
            catch (NotFoundResponseException)
            {
                userNotFound = true;
            }

            if (userNotFound || !await _accountBl.CheckPassword(user, userData.Password))
            {
                ModelState.AddModelError("loginFailure", "Invalid email or password");
                return BadRequest(ModelState);
            }

            var userClaims = await _tokenBl.GetClaimsAsync(user);
            var accessToken = _tokenBl.GenerateJwtAccessToken(userClaims);
            var refreshToken = _tokenBl.GenerateJwtRefreshToken();
            await _tokenBl.LoginByRefreshTokenAsync(user.Id, refreshToken);
            var tokens = new AuthTokensDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpireOn = _tokenBl.ExpirationTime
            };

            return Ok(tokens);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]AuthTokensDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var principal = _tokenBl.GetPrincipalFromExpiredAccessToken(dto.AccessToken);

            var user = await _accountBl.GetUserById(principal.Claims.Single(claim => claim.Type == "uid").Value); 

            var userClaims = await _tokenBl.GetClaimsAsync(user);

            dto.AccessToken = _tokenBl.GenerateJwtAccessToken(userClaims);
            dto.RefreshToken = await _tokenBl.UpdateRefreshTokenAsync(dto.RefreshToken, principal);
            dto.ExpireOn = _tokenBl.ExpirationTime;

            return Ok(dto);
        }

        [AllowAnonymous]
        [HttpPost("forget")]
        public async Task<IActionResult> Forget([FromBody]AccountMinimalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _accountBl.GetUserByEmail(dto.Email);
                await _notificationBl.SendPasswordResetNotification(user, await _accountBl.GeneratePasswordResetTokenAsync(user));
            }
            catch (NotFoundResponseException)
            {
                return BadRequest($"There is no User with email \"{dto.Email}\"!");
            }
            catch(InvalidOperationException)
            {
                return BadRequest($"Ooops... There are more than one User with that email, contact with Administrator to restore your password.");
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody]AuthTokensDto dto)
        {
            var principal = _tokenBl.GetPrincipalFromExpiredAccessToken(dto.AccessToken);
            await _tokenBl.DeleteRefreshTokenAsync(principal);

            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateDto userDto, [FromRoute]string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User userModel = await _accountBl.FindByIdAsync(userId);

            userModel.UserName = userDto.UserName;

            await _accountBl.UpdateAsync(userModel);
            return Ok("User updated");
        }



        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/add")]
        public async Task<IActionResult> AddRole([FromBody]RoleDto dto)
        {
            if (await _accountBl.RoleExistsAsync(dto.Role))
                return BadRequest($"Role \"{dto.Role}\" already exist");
            await _accountBl.CreateRoleAsync(new IdentityRole(dto.Role));

            return Ok($"Role \"{dto.Role}\" successfully added");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/remove")]
        public async Task<IActionResult> DeleteRole([FromBody]RoleDto dto)
        {
            if (!await _accountBl.RoleExistsAsync(dto.Role))
                return BadRequest($"Role \"{dto.Role}\" doesn`t exist");
            var role = await _accountBl.FindRoleByNameAsync(dto.Role);
            await _accountBl.DeleteRoleAsync(role); 
            return Ok($"Role \"{dto.Role}\" successfully removed away from this project");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/promote-to-admin/{userId}")]
        public async Task<IActionResult> PromoteToAdmin([FromRoute]string userId)
        {
            return await PromoteUser(new UserDemotionDto()
            {
                Role = "Administrator",
                UserId = userId
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/promote-to-user/{userId}")]
        public async Task<IActionResult> PromoteToUser([FromRoute]string userId)
        {
            return await PromoteUser(new UserDemotionDto()
            {
                Role = "User",
                UserId = userId
            });
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/demote-from-admin/{userId}")]
        public async Task<IActionResult> DemoteFromAdmin([FromRoute]string userId)
        {
            return await DemoteUser(new UserDemotionDto()
            {
                Role = "Administrator",
                UserId = userId
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/demote-from-user/{userId}")]
        public async Task<IActionResult> DemoteFromUser([FromRoute]string userId)
        {
            return await DemoteUser(new UserDemotionDto()
            {
                Role = "User",
                UserId = userId
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/promote-user")]
        public async Task<IActionResult> PromoteUser([FromBody]UserDemotionDto dto)
        {
            User user = await _accountBl.GetUserById(dto.UserId);
            if (user == null)
                return BadRequest($"User with id: {dto.UserId} not found");

            if (!await _accountBl.RoleExistsAsync(dto.Role))
                return BadRequest($"Role \"{dto.Role}\" not found");

            if (await _accountBl.IsInRoleAsync(user, dto.Role))
                return BadRequest($"User already Promoted to Role \"{dto.Role}\"");

            await _accountBl.AddToRoleAsync(user, dto.Role);
            await _accountBl.UpdateAsync(user);

            return Ok($"User \"{dto.UserId}\" successfully promoted to Role \"{dto.Role}\"");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("roles/demote-user")]
        public async Task<IActionResult> DemoteUser([FromBody]UserDemotionDto dto)
        {
            User user = await _accountBl.GetUserById(dto.UserId);
            if (user == null)
                return BadRequest($"User with id: {dto.UserId} not found");

            if (!await _accountBl.RoleExistsAsync(dto.Role))
                return BadRequest($"Role \"{dto.Role}\" not found");

            if (!await _accountBl.IsInRoleAsync(user, dto.Role))
                return BadRequest($"User already Demoted from Role \"{dto.Role}\"");

            await _accountBl.RemoveFromRoleAsync(user, dto.Role);
            await _accountBl.UpdateAsync(user);
            return Ok($"User \"{dto.UserId}\" successfully demoted from Role \"{dto.Role}\"");
        }


        [Authorize(Roles = "User")]
        [HttpPut]
        [Route("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordDto chPassDto, [FromRoute]string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (IsAdministrator)
            {
                await _accountBl.ChangePassword(userId, chPassDto.NewPassword);
                return new OkObjectResult("Password changed");
            }

            var user = await _accountBl.GetUserById(userId);
            if (!await _accountBl.CheckPassword(user, chPassDto.CurrentPassword))
                return BadRequest("Wrong password!");

            if (UserId == userId)
            {
                 await _accountBl.ChangePassword(userId, chPassDto.CurrentPassword, chPassDto.NewPassword);
                return Ok("Password changed");
            }
            else
                return BadRequest("Can not change for this user");
        }


        [Authorize(Roles = "User")]
        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoleById([FromRoute]string userId)
        {
            IList<string> userRoles;
            if (UserId == userId || IsAdministrator)
            {
                User user = await _accountBl.FindByIdAsync(userId);
                if (user == null)
                    return BadRequest("There is no User with that ID ");
                else
                    userRoles = await _accountBl.GetRolesAsync(user);
                return new OkObjectResult(userRoles);
            }
            else
                return BadRequest("Can not get roles for this user");
        }


        [AllowAnonymous]
        [HttpPut("{userId}/restore-password")]
        public async Task<IActionResult> RestorePassword([FromRoute]string userId, [FromBody]PasswordRestoreDto restoreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = await _accountBl.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("There is no User with that ID ");
            else
                await _accountBl.ResetPasswordAsync(user, restoreDto);
            return Ok("Password restored successfully.");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRegisterDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = _mapper.Map<AdminRegisterDto, User>(userDto);
            await _accountBl.CreateAdmin(user);
            return Ok("Admin created");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("page")]
        public async Task<ActionResult<FoundUsersPageDto>> GetAllUsers([FromQuery]int pageNumber = 1, int pageSize = 10)
        {
            var model = await _accountBl.GetUsersList(pageNumber, pageSize);

            var outputModel = new UsersPageDto()
            {
                Items = _mapper.Map<IEnumerable<User>, IEnumerable<UserMinimalDto>>(model.List),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = model.TotalItems
            };

            return new OkObjectResult(outputModel);
        }


    }
}