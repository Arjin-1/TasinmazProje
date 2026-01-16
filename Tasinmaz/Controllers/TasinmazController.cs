using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Dtos;
using System.IO;


namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/tasinmaz")]
    public class TasinmazController : ControllerBase
    {
        private readonly ITasinmazService _tasinmazService;

        public TasinmazController(ITasinmazService tasinmazService)
        {
            _tasinmazService = tasinmazService;
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Admin");

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var list = isAdmin
                ? await _tasinmazService.GetAllAsync()
                : await _tasinmazService.GetAllByUserAsync(userId);

            var dtoList = list.Select(t =>
            {
                string? geoJsonString = null;

                if (t.LocationGeometry != null)
                {
                    using var sw = new StringWriter();
                    using var jw = new JsonTextWriter(sw);

                    var serializer = GeoJsonSerializer.Create();
                    serializer.Serialize(jw, t.LocationGeometry);

                    geoJsonString = sw.ToString();
                }

                return new TasinmazListDto
                {
                    Id = t.Id,
                    Ada = t.Ada,
                    Parsel = t.Parsel,
                    Nitelik = t.Nitelik,
                    Address = t.Address,

                    MahalleId = t.MahalleId,
                    MahalleAd = t.Mahalle?.Ad,

                    IlceId = t.Mahalle?.Ilce?.Id,
                    IlceAd = t.Mahalle?.Ilce?.Ad,

                    IlId = t.Mahalle?.Ilce?.Il?.Id,
                    IlAd = t.Mahalle?.Ilce?.Il?.Ad,

                 
                    LocationGeometry = geoJsonString
                };
            }).ToList();

            return Ok(dtoList);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] TasinmazDto dto)
        {

            if (dto == null)
                return BadRequest("Gönderilen veri boş.");

            if (string.IsNullOrWhiteSpace(dto.LocationGeometry))
                return BadRequest("LocationGeometry zorunludur.");

           

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            Geometry geometry;

            try
            {
                geometry = _tasinmazService.GeoJsonToGeometry(dto.LocationGeometry);
                geometry.SRID = 4326;

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "GeoJSON parse edilemedi.",
                    detail = ex.InnerException?.Message ?? ex.Message
                });
            }

            var entity = new Tasinmaz.Entities.Concrete.Tasinmaz
            {
                Ada = dto.Ada,
                Parsel = dto.Parsel,
                Nitelik = dto.Nitelik,
                Address = dto.Address,
                MahalleId = dto.MahalleId,
                UserId = userId,
                LocationGeometry = geometry,
                
            };

            await _tasinmazService.AddAsync(entity);

            return Ok(new
            {
                message = "Taşınmaz başarıyla eklendi.",
                id = entity.Id,
                locationGeometry = dto.LocationGeometry
            });
        }



        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] TasinmazDto dto)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var entity = await _tasinmazService.GetByIdAsync(dto.Id);
            if (entity == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (entity.UserId != userId && !isAdmin)
                return Forbid();

            entity.Ada = dto.Ada;
            entity.Parsel = dto.Parsel;
            entity.Nitelik = dto.Nitelik;
            entity.MahalleId = dto.MahalleId;
            entity.Address = dto.Address;

            Geometry geometry;
            if (!string.IsNullOrWhiteSpace(dto.LocationGeometry))
            {
                try
                {
                    geometry = _tasinmazService.GeoJsonToGeometry(dto.LocationGeometry);
                }
                catch (Exception ex)
                {
                    return BadRequest(new
                    {
                        message = "GeoJSON parse edilemedi",
                        detail = ex.InnerException?.Message ?? ex.Message
                    });
                }
            }

            await _tasinmazService.UpdateAsync(entity);
            return Ok(dto);
        }


        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                return Unauthorized();

            var entity = await _tasinmazService.GetByIdAsync(id);
            if (entity == null)
                return Ok();

            var isAdmin = User.IsInRole("Admin");
            if (entity.UserId != userId && !isAdmin)
                return Forbid();

            await _tasinmazService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("Silinecek ID bulunamadı.");

            await _tasinmazService.DeleteMultipleAsync(ids);
            return Ok(new { message = "Seçili taşınmazlar silindi." });
        }

        [HttpPost("export/excel")]
        [Authorize]
        public async Task<IActionResult> ExportToExcel([FromBody] List<int>? selectedIds)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            int? userId = isAdmin
                ? null
                : int.TryParse(userIdString, out int uid) ? uid : null;

            if (!isAdmin && userId == null)
                return Unauthorized("Kullanıcı kimliği alınamadı.");

            byte[] fileBytes;

            if (selectedIds != null && selectedIds.Count > 0)
            {
                fileBytes = await _tasinmazService
                    .ExportTasinmazlarToExcelAsync(userId, selectedIds);
            }
            else
            {
                fileBytes = await _tasinmazService
                    .ExportTasinmazlarToExcelAsync(userId);
            }

            if (fileBytes == null || fileBytes.Length == 0)
                return NotFound("Aktarılacak taşınmaz bulunamadı.");

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Tasinmazlar.xlsx"
            );
        }

        [HttpPost("export/pdf")]
        [Authorize]
        public async Task<IActionResult> ExportToPdf([FromBody] List<int>? selectedIds)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            int? userId = isAdmin
                ? null
                : int.TryParse(userIdString, out int uid) ? uid : null;

            if (!isAdmin && userId == null)
                return Unauthorized("Kullanıcı kimliği alınamadı.");

            byte[] fileBytes;

            if (selectedIds != null && selectedIds.Count > 0)
            {
                fileBytes = await _tasinmazService
                    .ExportTasinmazlarToPdfAsync(userId, selectedIds);
            }
            else
            {
                fileBytes = await _tasinmazService
                    .ExportTasinmazlarToPdfAsync(userId);
            }

            if (fileBytes == null || fileBytes.Length == 0)
                return NotFound("Aktarılacak taşınmaz bulunamadı.");

            return File(
                fileBytes,
                "application/pdf",
                "Tasinmazlar.pdf"
            );
        }

        [HttpGet("pins")]
        [Authorize]
        public async Task<IActionResult> GetPins()
        {
            var isAdmin = User.IsInRole("Admin");
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);

 
            var list = isAdmin
                ? await _tasinmazService.GetAllAsync()
                : await _tasinmazService.GetAllByUserAsync(userId);

            var dtoList = list
                .Where(t => t.LocationGeometry != null)
                .Select(t => new
                {
                    id = t.Id,


                    locationGeometry = _tasinmazService.SerializeGeometryToGeoJson(
            t.LocationGeometry!
        )
                })
                .ToList();

            return Ok(dtoList);
        }


    }
}




















