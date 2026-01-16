using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasinmaz.Entities.Concrete
{
    public class Tasinmaz
    {
        [Key]
        public int Id { get; set; }
        public string Ada { get; set; } = null!;
        public string Parsel { get; set; } = null!;
        public string? Nitelik { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string? Address { get; set; }

        public Geometry? LocationGeometry { get; set; }

        [ForeignKey("Mahalle")]
        public int MahalleId { get; set; }
        public Mahalle Mahalle { get; set; } = null!;




    }
}


