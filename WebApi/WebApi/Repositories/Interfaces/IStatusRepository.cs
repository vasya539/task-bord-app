using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IStatusRepository
    {
        Task CreateAsync(Status status);
        Task<Status> ReadAsync(int id);
        Task UpdateAsync(Status status);
        Task DeleteAsync(int id);
        Task<List<Status>> GetAllAsync();
    }
}
