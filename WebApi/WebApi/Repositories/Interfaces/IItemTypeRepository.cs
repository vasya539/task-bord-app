using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IItemTypeRepository
    {
        Task CreateAsync(ItemType itemType);
        Task<ItemType> ReadAsync(int id);
        Task UpdateAsync(ItemType itemType);
        Task DeleteAsync(int id);
        Task<List<ItemType>> GetAllAsync();
    }
}
