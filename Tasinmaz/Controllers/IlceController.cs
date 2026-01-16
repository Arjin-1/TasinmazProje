using Microsoft.AspNetCore.Mvc;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Business.Concrete;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IlceController : ControllerBase
    {
        private readonly IIlceService _ilceService;

        public IlceController(IIlceService ilceService)
        {
            _ilceService = ilceService;
        }

        
        [HttpGet("getIlceByIlId/{ilId}")]
        public async Task<IActionResult> GetIlceByIlId(int ilId)
        {
            var ilceler = await _ilceService.GetByIlIdAsync(ilId);
            return Ok(ilceler);
        }
    }
}







