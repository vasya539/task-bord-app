using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Extensions.AddictionEnumerations;
using WebApi.Interfaces.IRepositories;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    /// <summary>
    /// Class for item repository that uses EF Core.
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor for repository, initialize context
        /// </summary>
        /// <param name="context">Context for database</param>
        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD-Operations

        /// <summary>
        /// Get all items from database.
        /// </summary>
        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            return await _context.Items
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .ToListAsync();
        }

        /// <summary>
        /// Get specific item from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Item object</returns>
        public async Task<Item> ReadAsync(int id)
        {
            return await _context.Items.AsNoTracking()
                .Where(r => r.Id == id)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.Parent)
                .Include(r => r.User)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Create a new item in database.
        /// </summary>
        /// <param name="item">Item to create</param>
        public async Task CreateAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update specific item in database.
        /// </summary>
        /// <param name="item">Item to update</param>
        public async Task UpdateAsync(Item item)
        {
            Console.WriteLine(_context.ChangeTracker.Entries().Count());
            _context.Entry(item).State = EntityState.Modified;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete specific item from database.
        /// </summary>
        /// <param name="id">Item to delete</param>
        public async Task DeleteAsync(int id)
        {
            var item = _context.Set<Item>().Find(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        #endregion CRUD-Operations

        /// <summary>
        /// Get all items for specific sprint
        /// </summary>
        /// <param name="sprintId">Sprint id</param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetBySprintIdAsync(int sprintId)
        {
            return await _context.Items.Where(r => r.SprintId == sprintId)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.User)
                .ToListAsync();
        }

        /// <summary>
        /// Get all arcived items for specific sprint
        /// </summary>
        /// <param name="sprintId">Sprint id</param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetArchivedBySprintIdAsync(int sprintId)
        {
            return await _context.Items.Where(r => r.SprintId == sprintId && r.IsArchived == true)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .ToListAsync();
        }

        /// <summary>
        /// Get all child for specific item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetAllChildAsync(int itemId)
        {
            return await _context.Items.Where(r => r.ParentId == itemId)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.User)
                .Include(r => r.Parent).AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Get all user-stories for specific sprint
        /// </summary>
        /// <param name="sprintId"></param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetUserStoriesAsync(int sprintId)
        {
            return await _context.Items
                .Where(r => r.TypeId == (int)ItemTypes.UserStory && r.SprintId == sprintId)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.User)
                .ToListAsync();
        }

        /// <summary>
        /// Get all unparented items for specific sprint
        /// </summary>
        /// <param name="sprintId"></param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetUnparentedAsync(int sprintId)
        {
            return await _context.Items
                .Where(r => r.ParentId == null && r.SprintId == sprintId && r.IsArchived == false && r.TypeId != (int)ItemTypes.UserStory)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.User)
                .ToListAsync();
        }

        /// <summary>
        /// Get all child for with specific status for specific item
        /// </summary>
        /// <param name="itemId">Parent id</param>
        /// <param name="statusId"> Status id</param>
        /// <returns>List of items</returns>
        public async Task<IEnumerable<Item>> GetChildWithSpecificStatusAsync(int itemId, int statusId)
        {
            return await _context.Items.Where(r => r.ParentId == itemId && r.StatusId == statusId && r.IsArchived == false)
                .Include(r => r.Status)
                .Include(r => r.ItemType)
                .Include(r => r.User)
                .Include(r => r.Parent)
                .ToListAsync();
        }
    }
}