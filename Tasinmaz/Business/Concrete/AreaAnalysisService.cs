using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Dtos;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Concrete
{
    public class AreaAnalysisService : IAreaAnalysisService
    {
        private readonly AppDbContext _context;
        private readonly WKTReader _wktReader;
        private readonly WKTWriter _wktWriter;

        public AreaAnalysisService(AppDbContext context)
        {
            _context = context;
            _wktReader = new WKTReader();
            _wktWriter = new WKTWriter();
        }

        public async Task<List<AnalysisGeometryDto>> GetBaseGeometriesAsync(int userId)
        {
            var list = await _context.AnalysisGeometry
                .Where(x => x.UserId == userId && !x.IsUnionResult && (x.Code == "A" || x.Code == "B" || x.Code == "C"))
                .OrderBy(x => x.Code)
                .ToListAsync();

            return list.Select(x => new AnalysisGeometryDto
            {
                Id = x.Id,
                Code = x.Code,
                Wkt = _wktWriter.Write(x.Geometry),
                AreaM2 = x.AreaM2,
                IsUnionResult = x.IsUnionResult
            }).ToList();
        }

        public async Task<List<AnalysisGeometryDto>> SaveBaseGeometriesAsync(
    int userId,
    List<GeometryInputDto> geometries)
        {
            if (geometries == null || geometries.Count == 0)
                throw new ArgumentException("No geometries sent.");

            var sentCodes = geometries
                .Select(g => g.Code.Trim().ToUpper())
                .OrderBy(c => c)
                .ToArray();

            string[] allowedSets2 = new[] { "A", "B" };
            string[] allowedSets3 = new[] { "A", "B", "C" };

            bool isTwoSet = sentCodes.SequenceEqual(allowedSets2);
            bool isThreeSet = sentCodes.SequenceEqual(allowedSets3);

            if (!isTwoSet && !isThreeSet)
            {
                throw new ArgumentException(
                    $"Please complete geometries {string.Join(", ", isTwoSet ? allowedSets2 : allowedSets3)}."
                );
            }

            var olds = await _context.AnalysisGeometry
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsUnionResult &&
                    sentCodes.Contains(x.Code))
                .ToListAsync();

            _context.AnalysisGeometry.RemoveRange(olds);

            var newEntities = new List<AnalysisGeometry>();

            foreach (var g in geometries)
            {
                if (string.IsNullOrWhiteSpace(g.Wkt))
                    throw new ArgumentException($"Geometry WKT for {g.Code} is empty.");

                Geometry geom;

                try
                {
                    geom = _wktReader.Read(g.Wkt);
                    geom.SRID = 4326;
                }
                catch
                {
                    throw new ArgumentException($"Geometri {g.Code} okunamadı (invalid WKT).");
                }

                if (geom == null || !geom.IsValid)
                    throw new ArgumentException($"Geometri {g.Code} geçersiz.");

                double area = geom.Area;

                newEntities.Add(new AnalysisGeometry
                {
                    UserId = userId,
                    Code = g.Code.ToUpper(),
                    Geometry = geom,
                    AreaM2 = area,
                    IsUnionResult = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.AnalysisGeometry.AddRangeAsync(newEntities);
            await _context.SaveChangesAsync();

            return newEntities
                .Select(x => new AnalysisGeometryDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Wkt = _wktWriter.Write(x.Geometry),
                    AreaM2 = x.AreaM2,
                    IsUnionResult = x.IsUnionResult
                })
                .ToList();
        }


        private async Task<(Geometry? A, Geometry? B, Geometry? C)> GetABC(int userId)
        {
            var geometries = await _context.AnalysisGeometry
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsUnionResult &&
                    (x.Code == "A" || x.Code == "B" || x.Code == "C"))
                .ToListAsync();

            var A = geometries.FirstOrDefault(x => x.Code == "A")?.Geometry;
            var B = geometries.FirstOrDefault(x => x.Code == "B")?.Geometry;
            var C = geometries.FirstOrDefault(x => x.Code == "C")?.Geometry;

            return (A, B, C);
        }


        public async Task<IntersectionResultDto> ComputeIntersectionAsync(int userId, string operation)
        {
            var (A, B, _) = await GetABC(userId);

            Geometry? result = null;

            switch (operation)
            {
                case "A_INTERSECT_B":
                    result = A.Intersection(B);
                    break;
                case "B_INTERSECT_A":
                    result = B.Intersection(A);
                    break;
                default:
                    throw new ArgumentException("Bilinmeyen kesişim işlemi.");
            }

            if (result == null || result.IsEmpty)
            {
                return new IntersectionResultDto
                {
                    HasIntersection = false,
                    Message = "Kesişim alanı yok."
                };
            }

            return new IntersectionResultDto
            {
                HasIntersection = true,
                Wkt = _wktWriter.Write(result),
                AreaM2 = result.Area,
                Message = "Kesişim başarıyla hesaplandı."
            };
        }

        public async Task<UnionResultDto> ComputeUnionAsync(int userId, string operation)
        {
            var (A, B, C) = await GetABC(userId);

         
            if (A == null || B == null)
                throw new ArgumentException("A ve B geometrileri zorunludur.");

            if (operation == "A_UNION_B_C" && C == null)
                throw new InvalidOperationException(
                    "A ∪ B ∪ C işlemi için C geometrisi çizilip kaydedilmelidir."
                );

            Geometry union;
            string code;

            switch (operation)
            {
                case "A_UNION_B":
                    union = A.Union(B);  
                    code = "D";
                    break;

                case "A_UNION_B_C":
                    if (C == null)
                        throw new ArgumentException("A ∪ B ∪ C işlemi için C geometrisi zorunludur.");

                    union = A.Union(B).Union(C); 
                    code = "E";
                    break;

                default:
                    throw new ArgumentException("Geçersiz birleşim işlemi.");
            }

            union.SRID = 4326;


            if (union == null || union.IsEmpty)
                throw new InvalidOperationException("Birleşim sonucu boş.");

            var area = union.Area;

            var entity = new AnalysisGeometry
            {
                UserId = userId,
                Code = code,
                Geometry = union,        
                AreaM2 = area,
                IsUnionResult = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.AnalysisGeometry.Add(entity);
            await _context.SaveChangesAsync();

            return new UnionResultDto
            {
                Code = code,
                Wkt = _wktWriter.Write(union),  
                AreaM2 = area
            };
        }

    }
}
