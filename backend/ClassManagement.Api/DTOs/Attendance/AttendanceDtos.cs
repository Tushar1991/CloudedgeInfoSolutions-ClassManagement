using System.ComponentModel.DataAnnotations;

namespace ClassManagement.Api.DTOs.Attendance;

public class MarkAttendanceRequest
{
    [Required]
    public string ClassId { get; set; } = string.Empty;

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool IsPresent { get; set; }

    [MaxLength(300)]
    public string? Notes { get; set; }
}

public class BulkAttendanceRequest
{
    [Required]
    public string ClassId { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required, MinLength(1)]
    public List<StudentAttendanceItem> Students { get; set; } = [];
}

public class StudentAttendanceItem
{
    [Required]
    public string StudentId { get; set; } = string.Empty;

    public bool IsPresent { get; set; }

    public string? Notes { get; set; }
}

public class AttendanceDto
{
    public string Id { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
