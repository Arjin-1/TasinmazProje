using System.ComponentModel.DataAnnotations;

namespace Tasinmaz.Dtos
{
    public class UserForUpdateDto
    {
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = null!;

        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
