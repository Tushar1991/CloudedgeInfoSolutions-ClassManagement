using System.ComponentModel.DataAnnotations;

namespace ClassManagement.Api.DTOs.Classes;

public class CreateClassRequest
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string TeacherId { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateClassRequest
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string TeacherId { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class ClassDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
