using Microsoft.EntityFrameworkCore;
using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Concrete
{
    public class MahalleService : IMahalleService
    {
        private readonly AppDbContext _context;

        public MahalleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mahalle>> GetAllAsync()
        {
            return await _context.Mahalle.Include(m => m.Ilce).ToListAsync();
        }

        public async Task<Mahalle?> GetByIdAsync(int id)
        {
            return await _context.Mahalle.FindAsync(id);
        }

        public async Task AddAsync(Mahalle entity)
        {
            await _context.Mahalle.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Mahalle entity)
        {
            _context.Mahalle.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mahalle = await _context.Mahalle.FindAsync(id);
            if (mahalle != null)
            {
                _context.Mahalle.Remove(mahalle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Mahalle>> GetByIlceIdAsync(int ilceId)
        {
            return await _context.Mahalle
                .Where(m => m.IlceId == ilceId)
                .ToListAsync();
        }
    }
}

