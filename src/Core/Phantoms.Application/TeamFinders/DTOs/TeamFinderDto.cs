namespace Phantoms.Application.TeamFinders.DTOs;

public class TeamFinderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? RequiredSkills { get; set; }
    public int NeededMembersCount { get; set; }
    public bool IsActive { get; set; }
    public Guid StudentId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTeamFinderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? RequiredSkills { get; set; }
    public int NeededMembersCount { get; set; }
    public Guid StudentId { get; set; }
}

public class UpdateTeamFinderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? RequiredSkills { get; set; }
    public int NeededMembersCount { get; set; }
    public bool IsActive { get; set; }
}