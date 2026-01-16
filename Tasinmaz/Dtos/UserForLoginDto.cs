using System.ComponentModel.DataAnnotations;

namespace Tasinmaz.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
