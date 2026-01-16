using NetTopologySuite.Geometries;

public class TasinmazFilterDTO
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? IlceId { get; set; }
    public int? MahalleId { get; set; }
    public string? Parsel { get; set; }
    public string? Ada { get; set; }
    public string? Address { get; set; }
    public int? OwnerId { get; set; }
    public string? LocationGeometry { get; set; }

}
