namespace RealEstateAgency.API.Dto;

public class AnnouncementEditRequest
{
    public Guid AnnouncementId { get; set; }
    public IFormFile? Photo { get; set; }
    public Guid PropertyType { get; set; }
    public Guid StatementType { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Area { get; set; }
    public int Floors { get; set; }
    public int Rooms { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}