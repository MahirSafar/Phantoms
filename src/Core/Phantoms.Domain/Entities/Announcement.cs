using Phantoms.Domain.Common;
using Phantoms.Domain.Enums;

namespace Phantoms.Domain.Entities;

public class Announcement : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementCategory Category { get; set; } = AnnouncementCategory.General;
    public PublishStatus Status { get; set; } = PublishStatus.Pending;
    public Guid TeacherId { get; set; }
    public AppUser? Teacher { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
}
