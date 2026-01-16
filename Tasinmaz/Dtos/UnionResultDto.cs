namespace Tasinmaz.Dtos
{
    public class UnionResultDto
    {
        public string Code { get; set; } = null!; // D veya E
        public string Wkt { get; set; } = null!;
        public double AreaM2 { get; set; }
    }
}
