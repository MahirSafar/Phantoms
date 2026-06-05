
using Phantoms.Domain.Common;

namespace Phantoms.Domain.Entities;

public class Student : AuditableEntity
{
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialty { get; set; }
    public string? Bio { get; set; }

    public ICollection<LostFound> LostFounds { get; set; } = new List<LostFound>();
    public ICollection<TeamFinder> TeamFinders { get; set; } = new List<TeamFinder>();
}