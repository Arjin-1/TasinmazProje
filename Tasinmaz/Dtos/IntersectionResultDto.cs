namespace Tasinmaz.Dtos
{
    public class IntersectionResultDto
    {
        public bool HasIntersection { get; set; }
        public string? Wkt { get; set; }
        public double? AreaM2 { get; set; }
        public string Message { get; set; } = null!;
    }
}
