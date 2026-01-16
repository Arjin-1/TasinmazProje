


//using NetTopologySuite.Geometries;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Tasinmaz.Dtos; 
//using Tasinmaz.Entities.Concrete;

//namespace Tasinmaz.Business.Abstract
//{
//    public interface ITasinmazService
//    {
//        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllAsync();
//        Task<Tasinmaz.Entities.Concrete.Tasinmaz?> GetByIdAsync(int id);
//        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetByMahalleIdAsync(int mahalleId);
//        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllByUserAsync(int userId);
//        Task<Tasinmaz.Entities.Concrete.Tasinmaz> AddAsync(Tasinmaz.Entities.Concrete.Tasinmaz entity);

//        Task UpdateAsync(Tasinmaz.Entities.Concrete.Tasinmaz entity);
//        Task DeleteAsync(int id);
//        Task DeleteMultipleAsync(List<int> ids);

//        Task<byte[]> ExportTasinmazlarToExcelAsync(int? userId = null);
//        Task<byte[]> ExportTasinmazlarToPdfAsync(int? userId = null);
//        byte[] ExportToExcel(List<Tasinmaz.Entities.Concrete.Tasinmaz> tasinmazList);
//        byte[] ExportToPdf(List<Tasinmaz.Entities.Concrete.Tasinmaz> tasinmazList, int? userId = null);





//    }
//}

using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Abstract
{
    public interface ITasinmazService
    {

        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllAsync();
        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllByUserAsync(int userId);
        Task<Tasinmaz.Entities.Concrete.Tasinmaz?> GetByIdAsync(int id);
        Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetByMahalleIdAsync(int mahalleId);

        Task<Tasinmaz.Entities.Concrete.Tasinmaz> AddAsync(Tasinmaz.Entities.Concrete.Tasinmaz entity);
        Task UpdateAsync(Tasinmaz.Entities.Concrete.Tasinmaz entity);
        Task DeleteAsync(int id);
        Task DeleteMultipleAsync(List<int> ids);


        Task<byte[]> ExportTasinmazlarToExcelAsync(
    int? userId = null,
    List<int>? selectedIds = null
);

        Task<byte[]> ExportTasinmazlarToPdfAsync(
            int? userId = null,
            List<int>? selectedIds = null
        );


        Geometry GeoJsonToGeometry(string geoJson);
        string SerializeGeometryToGeoJson(Geometry geometry);
    }
}

