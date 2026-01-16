using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Dtos;


namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AreaAnalysisController : ControllerBase
    {
        private readonly IAreaAnalysisService _areaService;

        public AreaAnalysisController(IAreaAnalysisService areaService)
        {
            _areaService = areaService;
        }

        private int GetUserId()
        {

            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

        [HttpGet("auto-select")]
        public async Task<IActionResult> GetBase()
        {
            var userId = GetUserId();
            var result = await _areaService.GetBaseGeometriesAsync(userId);

            if (result.Count == 0)
                return NotFound(new { message = "Kaydedilmiş geometri bulunamadı. Lütfen Manuel Çizim'i kullanın." });

            return Ok(result);
        }

        [HttpPost("manual-save")]
        public async Task<IActionResult> SaveBase([FromBody] SaveBaseRequestDto dto)
        {
            var userId = GetUserId();

     
            if (dto == null || dto.Geometries == null || dto.Geometries.Count == 0)
                return BadRequest(new { message = "Geometri bilgisi sağlanmadı." });

            try
            {
                var result = await _areaService.SaveBaseGeometriesAsync(userId, dto.Geometries);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
     
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
       
                return StatusCode(500, new { message = "Beklenmeyen bir hata oluştu.", detail = ex.Message });
            }
        }


        [HttpPost("intersection")]
        public async Task<IActionResult> Intersection([FromBody] OperationRequestDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Request body boş olamaz." });
            }

            if (string.IsNullOrWhiteSpace(dto.Operation))
            {
                return BadRequest(new { message = "Operation bilgisi zorunludur." });
            }

            var userId = GetUserId();

            var result = await _areaService.ComputeIntersectionAsync(userId, dto.Operation);

            return Ok(result);
        }



        [HttpPost("union")]
        public async Task<IActionResult> Union([FromBody] OperationRequestDto dto)
        {
            var userId = GetUserId();

            try
            {
                var result = await _areaService.ComputeUnionAsync(userId, dto.Operation);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

