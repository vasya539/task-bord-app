using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    /// <summary>
    /// Sprint repository interface with sprint managing operations.
    /// </summary>
    public interface ISprintRepository
    {
        Task CreateAsync(Sprint sprint);
        Task<Sprint> GetByIdAsync(int id);
        Task UpdateAsync(Sprint sprint);
        Task DeleteAsync(Sprint sprint);
        Task<IEnumerable<Sprint>> GetAllByProjectIdAsync(int projectId);
        Task<IEnumerable<Item>> GetAllSprintItemsAsync(int id);
    }
}
