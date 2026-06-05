using Phantoms.Domain.Enums;

namespace Phantoms.Application.Announcements.DTOs;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementCategory Category { get; set; }
    public PublishStatus Status { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.General;
}

public class UpdateAnnouncementDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.General;
}
