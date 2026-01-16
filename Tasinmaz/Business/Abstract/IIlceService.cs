using Tasinmaz.Entities.Concrete;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tasinmaz.Business.Abstract
{

    public interface IIlceService
    {
        Task<List<Ilce>> GetAllAsync();
        Task<Ilce?> GetByIdAsync(int id);
        Task AddAsync(Ilce entity);
        Task UpdateAsync(Ilce entity);
        Task DeleteAsync(int id);

        Task<List<Ilce>> GetByIlIdAsync(int ilId);
    }
}
