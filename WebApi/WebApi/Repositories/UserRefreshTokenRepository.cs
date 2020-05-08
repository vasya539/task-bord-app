using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Repositories
{
    public class UserRefreshTokenRepository:IUserRefreshTokenRepository<UserRefreshToken,int>
    {
        private readonly AppDbContext db;

        public UserRefreshTokenRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task CreateAsync(UserRefreshToken model)
        {
            await db.UserRefreshTokens.AddAsync(model);
            await SaveAsync();
        }

        public async Task DeleteAsync(int key)
        {
            var model = await GetAsync(key);

            if (model != null)
            {
                db.UserRefreshTokens.Remove(model);
                await SaveAsync();
            }
            else
            {
                throw new Exceptions.NotFoundResponseException();
            }
        }

        public async Task<UserRefreshToken> GetAsync(int key)
        {
            return await db.UserRefreshTokens.FindAsync(key);
        }

        public async Task<UserRefreshToken> GetByUserIdAsync(string userId)
        {
            return await db.UserRefreshTokens.FirstOrDefaultAsync(token => token.UserId == userId);
        }

        public async Task<IEnumerable<UserRefreshToken>> GetListAsync()
        {
            return await db.UserRefreshTokens.ToListAsync();
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserRefreshToken model)
        {
            db.UserRefreshTokens.Update(model);
            await SaveAsync();
        }
    }
}
