using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Data.DTOs;
using WebApi.BLs.Communication;

namespace WebApi.BLs.Interfaces
{
    /// <summary>
    /// Specific project business logic interface with CRUD operations.
    /// </summary>
    public interface IProjectBl
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(int id, string userId);
        Task<ProjectResponse> CreateAsync(ProjectDto projectDto, string userId);
        Task<ProjectResponse> UpdateAsync(ProjectDto projectDto, string userId);
        Task<ProjectResponse> DeleteAsync(int id, string userId);
    }
}
