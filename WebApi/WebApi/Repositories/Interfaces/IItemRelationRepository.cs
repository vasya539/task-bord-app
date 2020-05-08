using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    /// <summary>
    /// Interface for ItemRelation repository
    /// </summary>
    public interface IItemRelationRepository
    {
        Task<ItemRelation> GetRecordAsync(int firstItemId, int secondItemId);

        Task<List<ItemRelation>> GetRelatedItems(int itemId);

        Task CreateRecordAsync(ItemRelation relation);

        Task DeleteRecordAsync(ItemRelation relation);
    }
}