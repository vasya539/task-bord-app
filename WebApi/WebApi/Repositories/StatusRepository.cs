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
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDbContext _context;
        public StatusRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Status status)
        {
            if (status == null) return;
            try
            {
                _context.Set<Status>().Add(status);
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
                var status = _context.Set<Status>().Find(id);
                _context.Statuses.Remove(status);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<Status>> GetAllAsync()
        {
            return await _context.Statuses.ToListAsync();
        }

        public async Task<Status> ReadAsync(int id)
        {
            try
            {
                return await _context.Set<Status>().FindAsync(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task UpdateAsync(Status status)
        {
            try
            {
                _context.Entry(status).State = EntityState.Modified;
                _context.Set<Status>().Update(status);
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
