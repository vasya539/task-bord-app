using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.Interfaces.IServices
{
    /// <summary>
    /// Specific item business logic interface with CRUD operations.
    /// </summary>
    public interface IItemBl
    {
        Task<IEnumerable<ItemDto>> GetAllAsync();

        Task<ItemDto> ReadAsync(int id);

        Task<ItemResponse> CreateAsync(ItemDto item);

        Task<ItemResponse> UpdateAsync(ItemDto item);

        Task<ItemResponse> DeleteAsync(int id);
    }
}