namespace RealEstateAgency.Core.DTO;

public class AnnouncementFull
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string StatementTypeName { get; set; }
    public required string PropertyTypeName { get; set; }
    public required decimal Price { get; set; }
    public required string Location { get; set; }
    public required double Area { get; set; }
    public required string Photo { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int Floors { get; set; }
    public required int Rooms { get; set; }
    public required string Description { get; set; }
    public Guid AuthorId { get; set; }
    public required string Author { get; set; }
    public bool IsVerified { get; set; }
}