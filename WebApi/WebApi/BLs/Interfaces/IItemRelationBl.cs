using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Data.Models;

namespace WebApi.BLs.Interfaces
{
    /// <summary>
    /// Specific ItemRelation business logic interface with CRUD operations.
    /// </summary>
    public interface IItemRelationBl
    {
        Task<ItemRelation> GetRecordAsync(int firstItemId, int secondItemId);

        Task<IEnumerable<ItemDto>> GetRelatedItemsAsync(int itemId);

        Task<ItemResponse> CreateRecordAsync(int firstItemId, int secondItemId, string userId);

        Task<ItemResponse> DeleteRecordAsync(int firstItemId, int secondItemId, string userId);
    }
}