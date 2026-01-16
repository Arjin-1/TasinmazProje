namespace Tasinmaz.Dtos
{
    public class TasinmazPagedResponseDTO
    {
        public IEnumerable<TasinmazDto> Data { get; set; } = new List<TasinmazDto>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
