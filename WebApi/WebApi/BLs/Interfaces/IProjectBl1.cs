using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    public interface IProjectBl
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task Create(ProjectDto dto);
        Task<ProjectDto> Read(int id);
        Task Update(ProjectDto project);
        Task Delete(int id);
    }
}
