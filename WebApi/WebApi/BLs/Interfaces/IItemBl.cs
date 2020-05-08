using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    /// <summary>
    /// Specific item business logic interface with CRUD operations.
    /// </summary>
    public interface IItemBl
    {
        Task<IEnumerable<ItemDto>> GetAllAsync();

        Task<IEnumerable<ItemDto>> GetBySprintIdAsync(int sprintId);

        Task<IEnumerable<ItemDto>> GetArchivedBySprintIdAsync(int sprintId);

        Task<IEnumerable<ItemDto>> GetAllChildAsync(int itemId);

        Task<IEnumerable<ItemDto>> GetChildWithSpecificStatusAsync(int itemId, int statusId);

        Task<IEnumerable<ItemDto>> GetUserStoriesAsync(int sprintId);

        Task<ItemDto> ReadAsync(int id);

        Task<ItemResponse> CreateAsync(ItemDto item, string userId);

        Task<ItemResponse> UpdateAsync(ItemDto item, string userId);

        Task<ItemResponse> DeleteAsync(int id, string userId);

        Task<ItemResponse> ArchivingAsync(int id, string userId);

        Task<IEnumerable<ItemDto>> GetUnparentedAsync(int sprintId);
    }
}