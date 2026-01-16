using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasinmaz.Entities.Concrete
{
    public class Mahalle
    {
        [Key]
        public int Id { get; set; }
        public string Ad { get; set; } = null!;

        [ForeignKey("Ilce")]
        public int IlceId { get; set; }
        public Ilce Ilce { get; set; } = null!;
    }
}
