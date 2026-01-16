namespace Tasinmaz.Dtos
{
    public class SaveBaseRequestDto
    {
        public string Operation { get; set; } = null!; // A_INTERSECT_B, A_UNION_B, A_UNION_B_C
        public List<GeometryInputDto> Geometries { get; set; } = new();
    }
}
