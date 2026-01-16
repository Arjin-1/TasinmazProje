namespace Tasinmaz.Dtos
{
    public class LogFilterDTO
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public string? OperationType { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
        public string? UserIp { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
