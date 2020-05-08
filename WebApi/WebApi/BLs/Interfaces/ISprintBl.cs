using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    /// <summary>
    /// Sprint business logic interface with sprint managing operations.
    /// </summary>
    public interface ISprintBl
    {
        Task<SprintResponse> CreateAsync(SprintDto dto, string userId);
        Task<SprintResponse> UpdateAsync(SprintDto dto, string userId);
        Task<SprintResponse> DeleteAsync(int id, string userId);
        Task<SprintDto> GetByIdAsync(int id, string userId);
        Task<IEnumerable<SprintDto>> GetAllByProjectIdAsync(int projectId, string userId);
        Task<IEnumerable<ItemListDto>> GetAllSprintItemsAsync(int id, string userId);
    }
}
