using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;
using WebApi.Interfaces.IRepositories;

namespace WebApi.BLs.Interfaces
{
    public interface IStatusBl
    {
        Task<List<StatusDto>> GetAllAsync();
        Task Create(StatusDto status);

        Task<StatusDto> Read(int id);
        Task Update(StatusDto status);
        Task Delete(int id);
    }
}
