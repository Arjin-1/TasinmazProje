using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Abstract
{
    public interface IMahalleService
    {
        Task<List<Mahalle>> GetAllAsync();
        Task<Mahalle?> GetByIdAsync(int id);
        Task AddAsync(Mahalle entity);
        Task UpdateAsync(Mahalle entity);
        Task DeleteAsync(int id);

        
        Task<List<Mahalle>> GetByIlceIdAsync(int ilceId);
    }
}

