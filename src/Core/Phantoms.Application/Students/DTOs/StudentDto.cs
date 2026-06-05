namespace Phantoms.Application.Students.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialty { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStudentDto
{
    public Guid AppUserId { get; set; }
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialty { get; set; }
    public string? Bio { get; set; }
}

public class UpdateStudentDto
{
    public string? University { get; set; }
    public string? Faculty { get; set; }
    public string? Specialty { get; set; }
    public string? Bio { get; set; }
}