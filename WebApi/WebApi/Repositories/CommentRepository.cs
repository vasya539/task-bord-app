using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    /// <summary>
    /// Class that realize repository-pattern for comment entity.
    /// </summary>
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initialize context
        /// </summary>
        /// <param name="context">Context for database</param>
        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a new comment in db-table
        /// </summary>
        /// <param name="comment">Comment to create</param>
        public async Task CreateAsync(Comment comment)
        {
            await _context.Set<Comment>().AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete specific comment from db-table
        /// </summary>
        /// <param name="id">Comment id to delete </param>
        public async Task DeleteAsync(int id)
        {
            var comment = _context.Set<Comment>().Find(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <returns>List of comments</returns>
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .Include(r => r.User).ToListAsync();
        }

        /// <summary>
        /// Get all comments for specific item
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>List of comments</returns>
        public async Task<IEnumerable<Comment>> GetByItemIdAsync(int itemId)
        {
            return await _context.Comments.Where(r => r.ItemId == itemId).Include(r => r.User).ToListAsync();
        }

        /// <summary>
        /// Get specific comment
        /// </summary>
        /// <param name="id">Comment id</param>
        /// <returns>Comment object</returns>
        public async Task<Comment> ReadAsync(int id)
        {
            return await _context.Comments.Where(r => r.Id == id).Include(r => r.Item).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Update specific comment
        /// </summary>
        /// <param name="comment">Comment to update</param>
        public async Task UpdateAsync(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}