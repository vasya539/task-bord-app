using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    /// <summary>
    /// Class that realize repository-pattern for ItemRelation entity.
    /// </summary>
    public class ItemRelationRepository : IItemRelationRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        ///  Initialize context
        /// </summary>
        /// <param name="context">Context for database</param>
        public ItemRelationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get specific relation from database
        /// </summary>
        /// <param name="firstItemId">Id of first item</param>
        /// <param name="secondItemId">Id of second item</param>
        /// <returns>ItemRelation object</returns>
        public async Task<ItemRelation> GetRecordAsync(int firstItemId, int secondItemId)
        {
            return await _context.ItemsRelations.FindAsync(new object[] { firstItemId, secondItemId });
        }

        /// <summary>
        /// Create new relation in database
        /// </summary>
        /// <param name="relation">New Relation object</param>
        public async Task CreateRecordAsync(ItemRelation relation)
        {
            await _context.ItemsRelations.AddAsync(relation);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete specific relation from database
        /// </summary>
        /// <param name="relation">Specific relation to delete</param>
        public async Task DeleteRecordAsync(ItemRelation relation)
        {
            _context.ItemsRelations.Remove(relation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get related items for specific item. Search for two primary keys.
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>Relation object</returns>
        public async Task<List<ItemRelation>> GetRelatedItems(int itemId)
        {
            var items = await _context.ItemsRelations
                .Where(r => r.FirstItemId == itemId || r.SecondItemId == itemId)
                .ToListAsync();
            return items;
        }
    }
}