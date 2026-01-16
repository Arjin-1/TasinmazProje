using Microsoft.AspNetCore.Mvc;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Business.Concrete;
using Tasinmaz.Entities.Concrete;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IlController : ControllerBase
    {
        private readonly IIlService _ilService;

        public IlController(IIlService ilService)
        {
            _ilService = ilService;
        }




        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _ilService.GetAllAsync();
            return Ok(list);
        }
    }
}

