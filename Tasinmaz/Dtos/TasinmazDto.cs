
using NetTopologySuite.Geometries;

namespace Tasinmaz.Dtos
{
    public class TasinmazDto
    {
        public int Id { get; set; }

        public string Ada { get; set; } = null!;
        public string Parsel { get; set; } = null!;
        public string? Nitelik { get; set; }

        public int MahalleId { get; set; }
        public string Address { get; set; } = null!;

        public string? LocationGeometry { get; set; }

        public int UserId { get; set; }
    }
}

