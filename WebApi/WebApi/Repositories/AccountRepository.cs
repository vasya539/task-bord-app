using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Exceptions;


namespace WebApi.Repositories
{
    public class AccountRepository:IAccountRepository
    {
        private UserManager<User> userManager; 
        
        public AccountRepository(UserManager<User> user)
        {
            this.userManager = user;
        }

        public async Task<bool> CheckPassword(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        public async Task<User> GetUserByUserName(string userName)
        {
            User applicationUser = await userManager.FindByNameAsync(userName);
            if (applicationUser == null)
                throw new NotFoundResponseException("There is no User with such UserName to get.");
            else
                return applicationUser;
        }
        public async Task<User> GetAsync(string userid)
        {
            User applicationUser = await userManager.FindByIdAsync(userid);
            if (applicationUser == null)
                throw new NotFoundResponseException("There is no such UserID to get the User from.");
            else
                return applicationUser;
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            User applicationUser = await userManager.FindByEmailAsync(email);
            if (applicationUser == null)
                throw new NotFoundResponseException("There is no User with such Email to get.");
            else
                return applicationUser;
        }
        public async Task ChangePassword(User user, string currentpassword, string newpassword)
        {
            ChechkIdentitySuccess(await userManager.ChangePasswordAsync(user, currentpassword, newpassword));
        }

        public async Task<IList<string>> GetUserRoles(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public Task<int> GetUsersCount()
        {
            return Task.FromResult(userManager.Users.Count());
        }

        #region AdminMethods

        public async Task ChangePassword(User user, string newpassword)
        {
            var passResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ChangePasswordAsync(user, passResetToken, newpassword);
        }

        public Task<IEnumerable<User>> GetListAsync()
        {
            return Task.FromResult(userManager.Users.AsEnumerable());
        }
        public Task<IEnumerable<User>> GetListAsync(int pageNumber, int pageSize)
        {
            return Task.FromResult(userManager.Users.Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .ToList().AsEnumerable());
        }
            #endregion



            #region HelpersMethods
            private void ChechkIdentitySuccess(IdentityResult identityResult)
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
        #endregion
    }
}
