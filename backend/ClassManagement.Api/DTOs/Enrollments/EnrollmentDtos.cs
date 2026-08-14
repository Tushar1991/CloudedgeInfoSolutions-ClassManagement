using System.ComponentModel.DataAnnotations;

namespace ClassManagement.Api.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    [Required]
    public string ClassId { get; set; } = string.Empty;

    [Required]
    public string StudentId { get; set; } = string.Empty;
}

public class EnrollmentDto
{
    public string Id { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}
