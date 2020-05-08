using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Models;
using WebApi.BLs.Communication;
using WebApi.Data.DTOs;

namespace WebApi.BLs.Interfaces
{
    public interface IItemTypeBl
    {
        Task<List<ItemTypeDto>> GetAllAsync();
        Task Create(ItemTypeDto itemTypeDto);

        Task<ItemTypeDto> Read(int id);
        Task Update(ItemTypeDto itemTypeDto);
        Task Delete(int id);
    }
}
