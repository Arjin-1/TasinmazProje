namespace Tasinmaz.Dtos
{
    public class GeometryInputDto
    {
        public string Code { get; set; } = null!;

        // WKT formatında geometri (Polygon)
        public string Wkt { get; set; } = null!;
    }
}
