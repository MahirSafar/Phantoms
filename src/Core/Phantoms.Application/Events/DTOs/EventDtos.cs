using Phantoms.Domain.Enums;

namespace Phantoms.Application.Events.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public PublishStatus Status { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}

public class UpdateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}
