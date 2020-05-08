using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Interfaces.IRepositories;

namespace WebApi.Repositories
{
    /// <summary>
    /// Class for project repository  
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Project> _dbSet;

        /// <summary>
        /// Constructor for repository, initialize context
        /// </summary>
        /// <param name="context">Context for database</param>
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Projects;
        }

        #region CRUD-Operations

        /// <summary>
        /// Get all projects from database.
        /// </summary>
        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Get specific project from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Project object</returns>
        public async Task<Project> GetByIdAsync(int id)
        {
            var project = await _dbSet
                .Include(pu => pu.ProjectsUsers)
                    .ThenInclude(u=>u.User)
                .Include(s=>s.Sprints)
                .FirstOrDefaultAsync(x=>x.Id == id);

            return project;
        }

        /// <summary>
        /// Create a new project in database.
        /// </summary>
        /// <param name="project">Project to create</param>
        public async Task CreateAsync(Project project)
        {
            _dbSet.Add(project);
             await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update specific project in database.
        /// </summary>
        /// <param name="project">Project to update</param>
        public async Task UpdateAsync(Project project)
        {
            _dbSet.Update(project);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete specific project from database.
        /// </summary>
        /// <param name="id">Project to delete</param>
        public async Task DeleteAsync(int id)
        {
             var project = _dbSet.Find(id);
             _dbSet.Remove(project);
             await _context.SaveChangesAsync();
        }

        #endregion CRUD-Operations
    }
}