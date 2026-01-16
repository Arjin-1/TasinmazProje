namespace Tasinmaz.Dtos
{
    public class IntersectionRequestDto
    {
        public string WktA { get; set; } = null!;
        public string WktB { get; set; } = null!;
        public string Operation { get; set; } = null!;
    }

}
