using Microsoft.AspNetCore.Mvc;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MahalleController : ControllerBase
    {
        private readonly IMahalleService _mahalleService;

        public MahalleController(IMahalleService mahalleService)
        {
            _mahalleService = mahalleService;
        }

        
        [HttpGet("getMahalleByIlceId/{ilceId}")]
        public async Task<IActionResult> GetMahalleByIlceId(int ilceId)
        {
            var mahalleler = await _mahalleService.GetByIlceIdAsync(ilceId);
            return Ok(mahalleler);
        }
    }
}

