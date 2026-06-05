using Phantoms.Domain.Common;

namespace Phantoms.Domain.Entities;

public class TeamFinder : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? ProjectName { get; set; }
    public string? RequiredSkills { get; set; }

    public int NeededMembersCount { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
}