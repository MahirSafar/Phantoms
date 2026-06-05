namespace Phantoms.Application.LostFounds.DTOs;

public class LostFoundDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFound { get; set; }
    public bool IsResolved { get; set; }
    public Guid StudentId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLostFoundDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFound { get; set; }
    public Guid StudentId { get; set; }
}

public class UpdateLostFoundDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFound { get; set; }
    public bool IsResolved { get; set; }
}