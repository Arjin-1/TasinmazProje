using System.ComponentModel.DataAnnotations;

namespace Tasinmaz.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        
        [Required]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Parola 8-12 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!@#\$%\^&\*]).{8,12}$",
            ErrorMessage = "Parola en az 1 harf, 1 rakam ve 1 özel karakter içermelidir.")]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;


    }
}
