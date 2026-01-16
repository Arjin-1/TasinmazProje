using Tasinmaz.Dtos;


namespace Tasinmaz.Business.Abstract
{
    public interface IAreaAnalysisService
    {
        Task<List<AnalysisGeometryDto>> GetBaseGeometriesAsync(int userId);
        Task<List<AnalysisGeometryDto>> SaveBaseGeometriesAsync(int userId, List<GeometryInputDto> geometries);

        Task<IntersectionResultDto> ComputeIntersectionAsync(int userId, string operation);
        Task<UnionResultDto> ComputeUnionAsync(int userId, string operation);
    }
}

