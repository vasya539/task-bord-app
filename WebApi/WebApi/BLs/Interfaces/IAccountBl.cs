using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.DTOs.AccountDtos;
using WebApi.Data.Models;

namespace WebApi.BLs.Interfaces
{
    public interface IAccountBl
    {
        Task<bool> CheckPassword(string password);
        Task<bool> CheckPassword(User user, string password);
        Task<User> GetUserByName(string userName);
        Task<User> GetUserById(string id);
        Task<User> GetUserByEmail(string email);
        Task ChangePassword(string userId, string currentpassword, string newpassword);
        Task ChangePassword(string userId, string newpassword);
        Task CreateAdmin(User user);
        Task<IList<string>> GetUserRoles(User user);
        Task<UsersPage<User>> GetUsersList(int pageNumber, int pageSize);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityRole> FindRoleByNameAsync(string role);
        Task CreateAsync(RegisterDto newUserData);
        Task<User> FindByIdAsync(string userId);
        Task UpdateAsync(User userModel);
        Task CreateRoleAsync(IdentityRole identityRole);
        Task DeleteRoleAsync(IdentityRole role);
        Task<bool> IsInRoleAsync(User user, string v);
        Task AddToRoleAsync(User user, string role);
        Task RemoveFromRoleAsync(User user, string role);
        Task<IList<string>> GetRolesAsync(User user);
        Task ResetPasswordAsync(User user, PasswordRestoreDto dto);
    }
}
