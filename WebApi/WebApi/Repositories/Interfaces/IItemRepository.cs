using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    /// <summary>
    /// Specific Item interface repository.
    /// </summary>
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();

        Task<IEnumerable<Item>> GetBySprintIdAsync(int sprintId);

        Task<IEnumerable<Item>> GetArchivedBySprintIdAsync(int sprintId);

        Task<IEnumerable<Item>> GetUserStoriesAsync(int sprintId);

        Task<IEnumerable<Item>> GetAllChildAsync(int itemId);

        Task<IEnumerable<Item>> GetChildWithSpecificStatusAsync(int itemId, int statusId);

        Task CreateAsync(Item item);

        Task<Item> ReadAsync(int id);

        Task UpdateAsync(Item item);

        Task DeleteAsync(int id);

        Task<IEnumerable<Item>> GetUnparentedAsync(int sprintId);
    }
}