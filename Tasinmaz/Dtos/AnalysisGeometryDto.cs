namespace Tasinmaz.Dtos
{
    public class AnalysisGeometryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Wkt { get; set; } = null!;
        public double AreaM2 { get; set; }
        public bool IsUnionResult { get; set; }
    }
}
