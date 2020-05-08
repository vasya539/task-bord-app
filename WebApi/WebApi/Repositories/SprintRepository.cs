using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Repositories.Interfaces;
using WebApi.Data.Models;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories
{
    /// <summary>
    /// Class for sprint repository that uses EF Core.
    /// </summary>
    public class SprintRepository : ISprintRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor for repository, initializes database context.
        /// </summary>
        /// <param name="context">Database context.</param>
        public SprintRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all sprints by project Id from database using Entity Framework Core.
        /// </summary>
        /// <param name="projectId">Id of project.</param>
        /// <returns>Collection of sprints of given project.</returns>
        public async Task<IEnumerable<Sprint>> GetAllByProjectIdAsync(int projectId)
        {
            return await _context.Sprints.Where(sp => sp.ProjectId==projectId).ToListAsync();
        }

        /// <summary>
        /// Gets all items of sprint from database using Entity Framework Core.
        /// </summary>
        /// <param name="id">Id of sprint.</param>
        /// <returns>Collection of items.</returns>
        public async Task<IEnumerable<Item>> GetAllSprintItemsAsync(int id)
        {
            return await _context.Items.Where(sp => sp.SprintId == id).
                Include(i => i.ItemType).Include(i => i.Status).Include(i => i.User).ToListAsync();
        }

        /// <summary>
        /// Creates new sprint in database using Entity Framework Core.
        /// </summary>
        /// <param name="sprint">Object of sprint.</param>
        public async Task CreateAsync(Sprint sprint)
        {       
                await _context.Sprints.AddAsync(sprint);
                await _context.SaveChangesAsync();                     
        }

        /// <summary>
        /// Gets sprint by Id from database using Entity Framework Core.
        /// </summary>
        /// <param name="id">Id of sprint</param>
        /// <returns>Object of sprint.</returns>
        public async Task<Sprint> GetByIdAsync(int id)
        {
            return await _context.Sprints.FindAsync(id);
        }

        /// <summary>
        /// Updates a sprint in database using Entity Framework Core.
        /// </summary>
        /// <param name="sprint">Object of sprint.</param>
        public async Task UpdateAsync(Sprint sprint)
        {
                _context.Sprints.Update(sprint);
                await _context.SaveChangesAsync();           
        }

        /// <summary>
        /// Deletes a sprint in database using Entity Framework Core.
        /// </summary>
        /// <param name="sprint">Object of sprint.</param>
        public async Task DeleteAsync(Sprint sprint)
        {
            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
        }
    }
}
