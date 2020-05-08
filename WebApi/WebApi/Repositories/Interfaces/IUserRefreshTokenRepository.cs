using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Repositories.Interfaces
{
    public interface IUserRefreshTokenRepository <TValue, TKey> where TValue : class
    {
        Task<TValue> GetByUserIdAsync(string userId);
        /// <summary>
        /// List all entries.
        /// </summary>
        Task<IEnumerable<TValue>> GetListAsync();

        /// <summary>
        /// Gets specified entry.
        /// </summary>
        Task<TValue> GetAsync(TKey key);

        /// <summary>
        /// Creates provided enitity in the storage.
        /// </summary>
        Task CreateAsync(TValue model);

        /// <summary>
        /// Updates storage with the provided entity.
        /// </summary>
        Task UpdateAsync(TValue model);

        /// <summary>
        /// Deletes specified entity.
        /// </summary>
        Task DeleteAsync(TKey key);

        /// <summary>
        /// Save changes to storage.
        /// </summary>
        Task SaveAsync();
    }
}
