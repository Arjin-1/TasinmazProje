using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Business.Concrete
{
    public class TasinmazService : ITasinmazService
    {
        private readonly AppDbContext _context;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TasinmazService(
            AppDbContext context,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
        }


        public int GetCurrentUserId()
        {
            var claim = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier);

            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        public string GetUserIp()
        {
            return _httpContextAccessor.HttpContext?
                .Connection?.RemoteIpAddress?.ToString() ?? "N/A";
        }



        public Geometry GeoJsonToGeometry(string geoJson)
        {
            if (string.IsNullOrWhiteSpace(geoJson))
                throw new Exception("GeoJSON boş olamaz.");

            try
            {
                geoJson = geoJson.Trim();

                if (geoJson.StartsWith("\"") && geoJson.EndsWith("\""))
                {
                    geoJson = geoJson.Trim('"').Replace("\\\"", "\"");
                }

                var reader = new GeoJsonReader();
                var geometry = reader.Read<Geometry>(geoJson);

                if (geometry == null)
                    throw new Exception("GeoJSON parse sonucu NULL");

                geometry.SRID = 4326;
                return geometry;
            }
            catch (Exception ex)
            {
                throw new Exception("GeoJSON geometry parse edilemedi.", ex);
            }
        }

        public string SerializeGeometryToGeoJson(Geometry geometry)
        {
            using var sw = new StringWriter();
            using var jw = new JsonTextWriter(sw);

            var serializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
            serializer.Serialize(jw, geometry);

            return sw.ToString();
        }



        public async Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllAsync()
        {
            return await _context.Tasinmaz
                .Include(t => t.User)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                        .ThenInclude(i => i.Il)
                .ToListAsync();
        }

        public async Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetAllByUserAsync(int userId)
        {
            return await _context.Tasinmaz
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                        .ThenInclude(i => i.Il)
                .ToListAsync();
        }

        public async Task<Tasinmaz.Entities.Concrete.Tasinmaz?> GetByIdAsync(int id)
        {
            return await _context.Tasinmaz
                .Include(t => t.User)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                        .ThenInclude(i => i.Il)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tasinmaz.Entities.Concrete.Tasinmaz>> GetByMahalleIdAsync(int mahalleId)
        {
            return await _context.Tasinmaz
                .Where(t => t.MahalleId == mahalleId)
                .Include(t => t.Mahalle)
                    .ThenInclude(m => m.Ilce)
                        .ThenInclude(i => i.Il)
                .ToListAsync();
        }

        public async Task<Tasinmaz.Entities.Concrete.Tasinmaz> AddAsync(
    Tasinmaz.Entities.Concrete.Tasinmaz entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.LocationGeometry == null)
                throw new Exception("LocationGeometry zorunludur.");

            entity.LocationGeometry.SRID = 4326;

            var userId = GetCurrentUserId();
            var userIp = GetUserIp();

            try
            {

                _context.Tasinmaz.Add(entity);
                await _context.SaveChangesAsync();

                await _logService.AddLogAsync(
                    userId: userId,
                    operationType: "AddProperty",
                    description: $"Yeni taşınmaz eklendi. ID: {entity.Id}, Parsel: {entity.Parsel}",
                    status: "Success",
                    userIp: userIp
                );

                return entity;
            }
            catch (Exception ex)
            {
                await _logService.AddLogAsync(
                    userId: userId,
                    operationType: "AddProperty",
                    description:
                        $"Taşınmaz eklenirken hata oluştu. Parsel: {entity.Parsel}. Hata: {ex.Message}",
                    status: "Failure",
                    userIp: userIp
                );

                throw; 
            }
        }

        public async Task UpdateAsync(Tasinmaz.Entities.Concrete.Tasinmaz entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.LocationGeometry == null)
                throw new Exception("LocationGeometry zorunludur.");

            entity.LocationGeometry.SRID = 4326;

            _context.Tasinmaz.Update(entity);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                userId: GetCurrentUserId(),
                operationType: "UpdateProperty",
                description: $"Taşınmaz güncellendi. ID: {entity.Id}",
                status: "Success",
                userIp: GetUserIp()
            );
        }


        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Tasinmaz.FindAsync(id);
            if (entity == null) return;

            _context.Tasinmaz.Remove(entity);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                userId: GetCurrentUserId(),
                operationType: "DeleteProperty",
                description: $"Taşınmaz silindi. ID: {id}",
                status: "Success",
                userIp: GetUserIp()
            );
        }



        public async Task DeleteMultipleAsync(List<int> ids)
        {
            var items = await _context.Tasinmaz
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            _context.Tasinmaz.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> ExportTasinmazlarToExcelAsync(
    int? userId = null,
    List<int>? selectedIds = null)
        {
            List<Tasinmaz.Entities.Concrete.Tasinmaz> list;

            if (selectedIds != null && selectedIds.Any())
            {
                list = await _context.Tasinmaz
                    .Where(t => selectedIds.Contains(t.Id))
                    .Include(t => t.Mahalle)
                        .ThenInclude(m => m.Ilce)
                            .ThenInclude(i => i.Il)
                    .ToListAsync();
            }
            else
            {
                list = userId.HasValue
                    ? await GetAllByUserAsync(userId.Value)
                    : await GetAllAsync();
            }

            return ExportToExcel(list);
        }


        public byte[] ExportToExcel(
    List<Tasinmaz.Entities.Concrete.Tasinmaz> list)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Taşınmazlar");

            ws.Cell(1, 1).Value = "İl";
            ws.Cell(1, 2).Value = "İlçe";
            ws.Cell(1, 3).Value = "Mahalle";
            ws.Cell(1, 4).Value = "Ada";
            ws.Cell(1, 5).Value = "Parsel";
            ws.Cell(1, 6).Value = "Nitelik";
            ws.Cell(1, 7).Value = "Adres";

            int row = 2;
            foreach (var t in list)
            {
                ws.Cell(row, 1).Value = t.Mahalle?.Ilce?.Il?.Ad ?? "";
                ws.Cell(row, 2).Value = t.Mahalle?.Ilce?.Ad ?? "";
                ws.Cell(row, 3).Value = t.Mahalle?.Ad ?? "";
                ws.Cell(row, 4).Value = t.Ada;
                ws.Cell(row, 5).Value = t.Parsel;
                ws.Cell(row, 6).Value = t.Nitelik;
                ws.Cell(row, 7).Value = t.Address;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportTasinmazlarToPdfAsync(
    int? userId = null,
    List<int>? selectedIds = null)
        {
            List<Tasinmaz.Entities.Concrete.Tasinmaz> list;

            if (selectedIds != null && selectedIds.Any())
            {
                list = await _context.Tasinmaz
                    .Where(t => selectedIds.Contains(t.Id))
                    .Include(t => t.Mahalle)
                        .ThenInclude(m => m.Ilce)
                            .ThenInclude(i => i.Il)
                    .ToListAsync();
            }
            else
            {
                list = userId.HasValue
                    ? await GetAllByUserAsync(userId.Value)
                    : await GetAllAsync();
            }

            return ExportToPdf(list);
        }

        public byte[] ExportToPdf(
    List<Tasinmaz.Entities.Concrete.Tasinmaz> list)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            for (int i = 0; i < 7; i++)
                                c.RelativeColumn();
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("İl").Bold();
                            h.Cell().Text("İlçe").Bold();
                            h.Cell().Text("Mahalle").Bold();
                            h.Cell().Text("Ada").Bold();
                            h.Cell().Text("Parsel").Bold();
                            h.Cell().Text("Nitelik").Bold();
                            h.Cell().Text("Adres").Bold();
                        });

                        foreach (var t in list)
                        {
                            table.Cell().Text(t.Mahalle?.Ilce?.Il?.Ad ?? "");
                            table.Cell().Text(t.Mahalle?.Ilce?.Ad ?? "");
                            table.Cell().Text(t.Mahalle?.Ad ?? "");
                            table.Cell().Text(t.Ada);
                            table.Cell().Text(t.Parsel);
                            table.Cell().Text(t.Nitelik);
                            table.Cell().Text(t.Address);
                        }
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}







