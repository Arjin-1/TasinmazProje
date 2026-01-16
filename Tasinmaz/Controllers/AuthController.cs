using Microsoft.AspNetCore.Mvc;
using Tasinmaz.Data;
using Tasinmaz.Dtos;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto registerDto)
        {
            try
            {
                var createdUser = await _authRepository.RegisterAsync(registerDto);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto loginDto)
        {
            var response = await _authRepository.LoginAsync(loginDto);

            if (response == null)
                return Unauthorized(new { message = "Email veya şifre hatalı." });

            return Ok(response);
        }
    }
}

