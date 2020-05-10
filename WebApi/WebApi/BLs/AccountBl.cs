using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Repositories.Interfaces;
using WebApi.Data.Models;
using WebApi.Data.DTOs.AccountDtos;
using WebApi.BLs.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace WebApi.BLs
{
    public class AccountBl : IAccountBl
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotificationBl _notificationBl;
        private readonly IAccountRepository _accountRepository;


        public AccountBl(
            IAccountRepository accountRepository,
            INotificationBl notificationBl,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _notificationBl = notificationBl;
            _roleManager = roleManager;
            _accountRepository = accountRepository;
            _userManager = userManager;
        }
        public async Task<bool> CheckPassword(string password)
        {
            return password.Any((ch) => char.IsUpper(ch)) && 
                password.Any((ch) => char.IsLower(ch) && 
                password.Any((ch) => char.IsDigit(ch)));
        }
        public async Task<bool> CheckPassword(User user, string password)
        {
            return await _accountRepository.CheckPassword(user, password);
        }

        public async Task<User> GetUserByName(string userName)
        {
            return await _accountRepository.GetUserByUserName(userName);
        }
        public async Task<User> GetUserById(string id)
        {
            return await _accountRepository.GetAsync(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _accountRepository.GetUserByEmailAsync(email);
        }

        public async Task ChangePassword(string userId, string currentpassword, string newpassword)
        {
            User user = await GetUserById(userId);
            await _accountRepository.ChangePassword(user, currentpassword, newpassword);
        }
        public async Task ChangePassword(string userId, string newpassword)
        {
            User user = await GetUserById(userId);
            await _accountRepository.ChangePassword(user, newpassword);
        }

        public async Task<IList<string>> GetUserRoles(User user)
        {
            return await _accountRepository.GetUserRoles(user);
        }

        public async Task CreateAsync(RegisterDto newUserData)
        {
            User user = new User { Email = newUserData.Email, UserName = newUserData.Name };
            // adding user
            var result = await _userManager.CreateAsync(user, newUserData.Password).ConfigureAwait(true);
            // add role "User" to created user
            if (result.Succeeded)
                return;
            else throw new Exception("Identity issue(s): " + string.Join(", ", result.Errors));
        }

        public async Task CreateAdmin(User user)
        {
            await _userManager.CreateAsync(user, GeneratePassword());
            user = await GetUserByName(user.UserName);//JIC
            await _userManager.AddToRolesAsync(user, new List<string> { "Administrator", "User" });
            await _notificationBl.SendPasswordResetNotification(user, await _userManager.GeneratePasswordResetTokenAsync(user));
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<UsersPage<User>> GetUsersList(int pageNumber, int pageSize)
        {
            IEnumerable<User> users = await _accountRepository.GetListAsync(pageNumber, pageSize);
            int countOfUser = await _accountRepository.GetUsersCount();
            UsersPage<User> pagedList = new UsersPage<User>(users, pageNumber, pageSize, countOfUser);
            return pagedList;
        }

        private string GeneratePassword()
        {
            return "QwErasdf123@!--";
        }

        public Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task<bool> RoleExistsAsync(string role)
        {
            return _roleManager.RoleExistsAsync(role);
        }
        public Task<IdentityRole> FindRoleByNameAsync(string role)
        {
            return _roleManager.FindByNameAsync(role);
        }
        public Task DeleteRoleAsync(IdentityRole role)
        {
            return _roleManager.DeleteAsync(role);
        }
        public Task CreateRoleAsync(IdentityRole role)
        {
            return _roleManager.CreateAsync(role);
        }

        public Task UpdateAsync(User userModel)
        {
            return _userManager.UpdateAsync(userModel);
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }
        public async Task AddToRoleAsync(User user, string role)
        {
            CheckIdentityResultUtil(await _userManager.AddToRoleAsync(user, role));
        }

        public async Task RemoveFromRoleAsync(User user, string role)
        {
            CheckIdentityResultUtil(await _userManager.RemoveFromRoleAsync(user, role));
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
           return await _userManager.GetRolesAsync(user);
        }

        public async Task ResetPasswordAsync(User user,PasswordRestoreDto dto)
        {
            CheckIdentityResultUtil(await _userManager.ResetPasswordAsync(user, dto.RestoreToken, dto.NewPassword));
        }


        private void CheckIdentityResultUtil(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
                return;

            List<string> exceptions = new List<string>();

            foreach (IdentityError item in identityResult.Errors)
            {
                exceptions.Add(item.Code);
            }
            throw new Exception("Identity issue(s): " + string.Join(", ", exceptions));
        }
    }
}

