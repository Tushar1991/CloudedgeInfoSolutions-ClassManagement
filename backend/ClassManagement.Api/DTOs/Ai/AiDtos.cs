namespace ClassManagement.Api.DTOs.Ai;

public class AttendanceInsightRequest
{
    public string? ClassId { get; set; }
}

public class AttendanceInsightResponse
{
    public string Summary { get; set; } = string.Empty;
    public double ClassAverageAttendancePercent { get; set; }
    public int TotalStudentsAnalyzed { get; set; }
    public int AtRiskCount { get; set; }
    public List<StudentAttendanceRiskDto> AtRiskStudents { get; set; } = [];
    public List<string> Suggestions { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class StudentAttendanceRiskDto
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public double AttendancePercent { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Warning { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
}

public class MarksFeedbackRequest
{
    public string MarkId { get; set; } = string.Empty;
}

public class MarksFeedbackResponse
{
    public string MarkId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public string PerformanceLevel { get; set; } = string.Empty;
}
