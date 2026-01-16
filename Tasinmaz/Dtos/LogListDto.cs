using System;

namespace Tasinmaz.Dtos
{
    public class LogListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OperationType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserIp { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
