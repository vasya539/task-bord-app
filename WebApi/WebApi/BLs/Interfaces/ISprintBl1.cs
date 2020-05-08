using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    public interface ISprintBl
    {
        Task<IEnumerable<SprintDto>> GetAllAsync();
        Task<SprintResponse> CreateAsync(SaveSprintDto dto);
        Task<SprintResponse> UpdateAsync(SprintDto dto);
        Task<SprintResponse> DeleteAsync(int id);
        Task<SprintDto> GetByIdAsync(int id);
    }
}
