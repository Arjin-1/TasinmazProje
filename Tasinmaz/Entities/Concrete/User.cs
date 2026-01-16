using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Tasinmaz.Entities.Concrete
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        
        [Required]
        public string PasswordHash { get; set; } = null!;


        [Required]
        public string Salt { get; set; } = null!;

        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
        public ICollection<Log> Logs { get; set; } = new List<Log>();


    }
}
