using System.ComponentModel.DataAnnotations;

namespace Tasinmaz.Entities.Concrete
{
    public class Il
    {
        [Key]
        public int Id { get; set; }
        public string Ad { get; set; } = null!;
    }
}
