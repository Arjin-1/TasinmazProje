using Microsoft.EntityFrameworkCore;
using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Concrete
{
    public class IlService : IIlService
    {
        private readonly AppDbContext _context;

        public IlService(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Il>> GetAllAsync()
        {
            return await _context.Iller.ToListAsync();
        }

       
        public async Task<Il?> GetByIdAsync(int id)
        {
            return await _context.Iller.FindAsync(id);
        }

        
        public async Task AddAsync(Il entity)
        {
            _context.Iller.Add(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Il entity)
        {
            _context.Iller.Update(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task DeleteAsync(int id)
        {
            var il = await _context.Iller.FindAsync(id);
            if (il != null)
            {
                _context.Iller.Remove(il);
                await _context.SaveChangesAsync();
            }
        }
    }
}

