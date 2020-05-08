using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Data.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly AppDbContext _context;

        public ItemTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ItemType itemType)
        {
            if (itemType == null) return;
            try
            {
                _context.Set<ItemType>().Add(itemType);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var itemType = _context.Set<ItemType>().Find(id);
                _context.ItemTypes.Remove(itemType);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<ItemType>> GetAllAsync()
        {
            return await _context.ItemTypes.ToListAsync();
        }

        public async Task<ItemType> ReadAsync(int id)
        {
            try
            {
                return await _context.Set<ItemType>().FindAsync(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task UpdateAsync(ItemType itemType)
        {
            try
            {
                _context.Entry(itemType).State = EntityState.Modified;
                _context.Set<ItemType>().Update(itemType);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
