using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<User> GetAsync(string id);
        Task<bool> CheckPassword(User user, string password);
        Task<User> GetUserByUserName(string userName);
        Task<User> GetUserByEmailAsync(string email);
        Task ChangePassword(User user, string currentpassword, string newpassword);
        Task ChangePassword(User user, string newpassword);
        Task<IList<string>> GetUserRoles(User user);
        Task<IEnumerable<User>> GetListAsync();
        Task<IEnumerable<User>> GetListAsync(int pageNumber, int pageSize);
        Task<int> GetUsersCount();
    }
}
