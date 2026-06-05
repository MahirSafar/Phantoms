using Phantoms.Domain.Common;

namespace Phantoms.Domain.Entities;

public class LostFound : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }
    public string? ImageUrl { get; set; }

    public bool IsFound { get; set; }
    public bool IsResolved { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
}