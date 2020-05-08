using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.BLs.Interfaces;
using WebApi.Controllers;
using WebApi.Data.DTOs.AccountDtos;
using WebApi.Data.DTOs.AccountDtos.Roles;
using WebApi.Data.Models;
using Xunit;

namespace WebApiTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<INotificationBl> mockNotificationBl;
        private readonly Mock<IUserBl> mockUserBl;
        private readonly Mock<IAccountBl> mockAccountBl;
        private readonly Mock<IJwtTokenBl> mockJwtTokenBl;
        private readonly Mock<IMapper> mockMapper;
        private readonly Mock<User> mockUser;
        private readonly Mock<LoginDto> mockLoginDto;
        private readonly Mock<RegisterDto> mockRegisterDto;

        public AccountControllerTests()
        {
            mockNotificationBl = new Mock<INotificationBl>();
            mockUserBl = new Mock<IUserBl>();
            mockAccountBl = new Mock<IAccountBl>();
            mockJwtTokenBl = new Mock<IJwtTokenBl>();
            mockMapper = new Mock<IMapper>();
            mockUser = new Mock<User>();
            mockLoginDto = new Mock<LoginDto>();
            mockRegisterDto = new Mock<RegisterDto>();
        }

        [Theory]
        [InlineData("token1", "token1")]
        [InlineData("token2", "token2")]
        [InlineData("token1", "token2")]
        [InlineData("token2", "token1")]
        [InlineData("somedata", "somedata")]
        public async Task Login_WithCorrectParameters_ReturnsJwtTokensAsync(string accessToken, string refreshToken)
        {
            var userClaims = It.IsAny<Claim[]>();
            var expectedAccessToken = accessToken;
            var expectedRefreshToken = refreshToken;
            var expectedTime = DateTime.Now.AddMinutes(120);
            var expectedTokens = new AuthTokensDto
            {
                AccessToken = expectedAccessToken,
                RefreshToken = expectedRefreshToken,
                ExpireOn = expectedTime
            };
            mockAccountBl.Setup(accountBl => accountBl.GetUserByName(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.CheckPassword(mockUser.Object, It.IsAny<string>())).ReturnsAsync(true);
            mockJwtTokenBl.Setup(jwtBl => jwtBl.GetClaimsAsync(mockUser.Object)).ReturnsAsync(userClaims);
            mockJwtTokenBl.Setup(jwtBl => jwtBl.GenerateJwtAccessToken(userClaims)).Returns(expectedAccessToken);
            mockJwtTokenBl.Setup(jwtBl => jwtBl.GenerateJwtRefreshToken()).Returns(expectedRefreshToken);
            mockJwtTokenBl.Setup(jwtBl => jwtBl.LoginByRefreshTokenAsync(Guid.NewGuid().ToString(), expectedRefreshToken));
            mockJwtTokenBl.Setup(jwtBl => jwtBl.ExpirationTime).Returns(expectedTime);


            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);
            var result = await AccountController.Login(new LoginDto()
            {
                Name="someName",
                Password="somePass"
            });


            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTokens = Assert.IsAssignableFrom<AuthTokensDto>(okResult.Value);
            Assert.Equal(expectedTokens, actualTokens);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserCannotBeFoundAsync()
        {
            mockAccountBl.Setup(accountBl => accountBl.GetUserByEmail(It.IsAny<string>())).Throws(new NullReferenceException());

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                    mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);
            
            Assert.IsType<BadRequestObjectResult>(await AccountController.Login(mockLoginDto.Object));
            mockAccountBl.Verify(accountBl => accountBl.CheckPassword(mockUser.Object, It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordNotMatchAsync()
        {
            mockAccountBl.Setup(accountBl => accountBl.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.CheckPassword(mockUser.Object, It.IsAny<string>())).ReturnsAsync(false);

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                    mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);
            var result = await AccountController.Login(mockLoginDto.Object);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_MustCreateNewUserAsync()
        {
            mockMapper.Setup(mapperBl => mapperBl.Map<User>(mockRegisterDto.Object)).Returns(mockUser.Object);

            mockAccountBl.Setup(accountBl => accountBl.CreateAsync(mockRegisterDto.Object)).Returns(Task.CompletedTask);
            mockAccountBl.Setup(accountBl => accountBl.GetUserByName(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>()));

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

             var result = await AccountController.Register(mockRegisterDto.Object);

            mockAccountBl.Verify(accountBl => accountBl.CreateAsync(mockRegisterDto.Object), Times.Once);
            mockAccountBl.Verify(accountBl => accountBl.AddToRoleAsync(mockUser.Object, "User"), Times.Once);
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Logout_MustDeleteRefreshTokenAsync()
        {
            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            var mockTokens = new Mock<AuthTokensDto>();
            mockJwtTokenBl.Setup(jwtBl => jwtBl.GetPrincipalFromExpiredAccessToken(mockTokens.Object.AccessToken)).Returns(mockClaimsPrincipal.Object);
            mockJwtTokenBl.Setup(jwtBl => jwtBl.DeleteRefreshTokenAsync(mockClaimsPrincipal.Object)).Returns(Task.CompletedTask);

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                    mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

            var result = await AccountController.Logout(mockTokens.Object);

            Assert.IsType<OkResult>(result);
            mockJwtTokenBl.Verify(jwtBl => jwtBl.DeleteRefreshTokenAsync(mockClaimsPrincipal.Object), Times.Once);
        }


        [Fact]
        public async void Get_OkObjectResult_WhenCalled_GetUserRoleByIdAsync()
        {
            mockAccountBl.Setup(accountBl => accountBl.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.GetRolesAsync(mockUser.Object)).ReturnsAsync(It.IsAny<IList<string>>());

            var AccountController = new Mock<AccountController>(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

            AccountController.SetupGet(accContoller => accContoller.UserId).Returns(It.IsAny<string>());

            // Act
            var result = await AccountController.Object.GetUserRoleById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Get_List_WhenCalled_GetUserRoleByIdAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.GetRolesAsync(mockUser.Object)).ReturnsAsync(GetRoles());

            var AccountController = new Mock<AccountController>(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

            AccountController.SetupGet(accContoller => accContoller.UserId).Returns(It.IsAny<string>());
            // Act
            var result = await AccountController.Object.GetUserRoleById(It.IsAny<string>()) as OkObjectResult;

            //Assert 
            var items = Assert.IsType<List<string>>(result.Value);
            Assert.Equal(2, items.Count);
            Assert.Equal(GetRoles(), items);
        }

        private IList<string> GetRoles()
        {
            List<string> roles = new List<string>();
            roles.Add("User");
            roles.Add("Admin");
            return roles;
        }

        [Fact]
        public async void Get_Exception_WhenCalled_GetUserRoleByIdAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.FindByIdAsync(It.IsAny<string>())).Throws(new Exception("Default User Exception"));

            var AccountController = new Mock<AccountController>(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

            AccountController.SetupGet(accContoller => accContoller.UserId).Returns(It.IsAny<string>());

            // Act and Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => AccountController.Object.GetUserRoleById(It.IsAny<string>()));
            Assert.Equal("Default User Exception", ex.Message);
        }

        [Fact]
        public async void Post_WhenCalled_AddUserRoleByUserIdAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.GetUserById(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockAccountBl.Setup(accountBl => accountBl.IsInRoleAsync(mockUser.Object, It.IsAny<string>())).ReturnsAsync(false);
            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>())).Returns(Task.CompletedTask);

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);

            // Act
            var result = await AccountController.PromoteUser(new UserDemotionDto()
            {
                UserId = It.IsAny<string>(),
                Role = It.IsAny<string>()
            }) as OkObjectResult;

            //Assert 
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void Post_BadRequest_WhenCalled_AddUserRoleByUserId_WithBadIdAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.GetUserById(It.IsAny<string>()));
            mockAccountBl.Setup(accountBl => accountBl.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockAccountBl.Setup(accountBl => accountBl.IsInRoleAsync(mockUser.Object, It.IsAny<string>())).ReturnsAsync(false);

            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>())).Throws(new Exception("Default User Exception"));

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);


            // Act and Assert
            var ex = await AccountController.PromoteUser(new UserDemotionDto()
            {
                UserId = It.IsAny<string>(),
                Role = It.IsAny<string>()
            });
            Assert.IsType<BadRequestObjectResult>(ex);
        }
        [Fact]
        public async void Get_BadRequest_WhenCalled_AddUserRoleByUserId_WithUnexistedRoleAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.GetUserById(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            mockAccountBl.Setup(accountBl => accountBl.IsInRoleAsync(mockUser.Object, It.IsAny<string>())).ReturnsAsync(false);

            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>())).Throws(new Exception("Default User Exception"));

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);


            // Act and Assert
            var ex = await AccountController.PromoteUser(new UserDemotionDto()
            {
                UserId = It.IsAny<string>(),
                Role = It.IsAny<string>()
            });
            Assert.IsType<BadRequestObjectResult>(ex);
        }
        [Fact]
        public async void Get_BadRequest_WhenCalled_AddUserRoleByUserId_WhenUserIsAlreadyInRoleAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.GetUserById(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockAccountBl.Setup(accountBl => accountBl.IsInRoleAsync(mockUser.Object, It.IsAny<string>())).ReturnsAsync(true);

            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>())).Throws(new Exception("Default User Exception"));

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);


            // Act and Assert
            var ex = await AccountController.PromoteUser(new UserDemotionDto()
            {
                UserId = It.IsAny<string>(),
                Role = It.IsAny<string>()
            });
            Assert.IsType<BadRequestObjectResult>(ex);
        }
        [Fact]
        public async void Get_Exception_WhenCalled_AddUserRoleByUserIdAsync()
        {
            // Arrange
            mockAccountBl.Setup(accountBl => accountBl.GetUserById(It.IsAny<string>())).ReturnsAsync(mockUser.Object);
            mockAccountBl.Setup(accountBl => accountBl.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            mockAccountBl.Setup(accountBl => accountBl.IsInRoleAsync(mockUser.Object, It.IsAny<string>())).ReturnsAsync(false);

            mockAccountBl.Setup(accountBl => accountBl.AddToRoleAsync(mockUser.Object, It.IsAny<string>())).Throws(new Exception("Default User Exception"));
            mockAccountBl.Setup(accountBl => accountBl.UpdateAsync(mockUser.Object)).Returns(Task.CompletedTask);

            AccountController AccountController = new AccountController(mockJwtTokenBl.Object, mockUserBl.Object,
                            mockAccountBl.Object, mockNotificationBl.Object, mockMapper.Object);


            // Act and Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => AccountController.PromoteUser(new UserDemotionDto()
            {
                UserId = It.IsAny<string>(),
                Role = It.IsAny<string>()
            }));
            Assert.IsType<Exception>(ex);
            Assert.Equal("Default User Exception", ex.Message);
        }

    }
}
