
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Dtos;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserForRegisterDto registerDto)
        {
            var user = await _userService.RegisterUserAsync(registerDto);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto updateDto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, updateDto);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok(new { message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            var file = await _userService.ExportUsersToExcelAsync();
            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Kullanicilar.xlsx"
            );
        }


        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportUsersToPdf()
        {
            var file = await _userService.ExportUsersToPdfAsync();
            return File(file, "application/pdf", "Kullanicilar.pdf");
        }





    }
}

