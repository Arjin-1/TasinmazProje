using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasinmaz.Entities.Concrete
{
    public class Ilce
    {
        [Key] 
        public int Id { get; set; }
        public string Ad { get; set; } = null!;

        [ForeignKey("Il")]
        public int IlId { get; set; }
        public Il Il { get; set; } = null!;
    }
}
