using NetTopologySuite.Geometries;

namespace Tasinmaz.Dtos
{
    public class TasinmazListDto
    {
        public int Id { get; set; }
        public string Ada { get; set; } = null!;
        public string Parsel { get; set; } = null!;
        public string? Nitelik { get; set; }

        public string Address { get; set; } = null!;

        public int MahalleId { get; set; }
        public string? MahalleAd { get; set; }

        public int? IlceId { get; set; }
        public string? IlceAd { get; set; }

        public int? IlId { get; set; }
        public string? IlAd { get; set; }

        public string? LocationGeometry { get; set; }

    }
}

