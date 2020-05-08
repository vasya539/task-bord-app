using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    /// <summary>
    /// Interface for comment repository
    /// </summary>
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();

        Task<IEnumerable<Comment>> GetByItemIdAsync(int itemId);

        Task CreateAsync(Comment comment);

        Task<Comment> ReadAsync(int id);

        Task UpdateAsync(Comment comment);

        Task DeleteAsync(int id);
    }
}