using Microsoft.EntityFrameworkCore;
using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Concrete
{
    public class IlceService : IIlceService
    {
        private readonly AppDbContext _context;

        public IlceService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<Ilce>> GetAllAsync()
        {
            return await _context.Ilceler
                .Include(i => i.Il) 
                .ToListAsync();
        }

        
        public async Task<Ilce?> GetByIdAsync(int id)
        {
            return await _context.Ilceler
                .Include(i => i.Il)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        
        public async Task AddAsync(Ilce entity)
        {
            _context.Ilceler.Add(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Ilce entity)
        {
            _context.Ilceler.Update(entity);
            await _context.SaveChangesAsync();
        }

        
        public async Task DeleteAsync(int id)
        {
            var ilce = await _context.Ilceler.FindAsync(id);
            if (ilce != null)
            {
                _context.Ilceler.Remove(ilce);
                await _context.SaveChangesAsync();
            }
        }

       
        public async Task<List<Ilce>> GetByIlIdAsync(int ilId)
        {
            return await _context.Ilceler
                .Where(i => i.IlId == ilId)
                .Include(i => i.Il)
                .ToListAsync();
        }
    }
}

