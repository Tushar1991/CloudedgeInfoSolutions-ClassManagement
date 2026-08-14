using System.ComponentModel.DataAnnotations;

namespace ClassManagement.Api.DTOs.Marks;

public class CreateMarkRequest
{
    [Required]
    public string ClassId { get; set; } = string.Empty;

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(100)]
    public string ExamName { get; set; } = string.Empty;

    [Required, Range(0, 1000)]
    public decimal Score { get; set; }

    [Range(1, 1000)]
    public decimal MaxScore { get; set; } = 100;
}

public class UpdateMarkRequest
{
    [Required, MinLength(2), MaxLength(100)]
    public string ExamName { get; set; } = string.Empty;

    [Required, Range(0, 1000)]
    public decimal Score { get; set; }

    [Range(1, 1000)]
    public decimal MaxScore { get; set; } = 100;
}

public class MarkDto
{
    public string Id { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Percentage => MaxScore == 0 ? 0 : Math.Round(Score / MaxScore * 100, 2);
    public string? Feedback { get; set; }
    public DateTime RecordedAt { get; set; }
}
