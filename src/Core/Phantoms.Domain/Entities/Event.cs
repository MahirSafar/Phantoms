using Phantoms.Domain.Common;
using Phantoms.Domain.Enums;

namespace Phantoms.Domain.Entities;

public class Event : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.Pending;
    public Guid TeacherId { get; set; }
    public AppUser? Teacher { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
}
