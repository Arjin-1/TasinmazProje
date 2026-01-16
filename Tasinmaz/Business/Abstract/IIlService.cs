

using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Abstract
{
   
    public interface IIlService
    {
        Task<List<Il>> GetAllAsync();
        Task<Il?> GetByIdAsync(int id);
        Task AddAsync(Il entity);
        Task UpdateAsync(Il entity);
        Task DeleteAsync(int id);
    }
}

