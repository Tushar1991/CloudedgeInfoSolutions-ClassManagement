using ClassManagement.Api.DTOs.Ai;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

/// <summary>
/// AI-powered attendance risk analysis and smart marks feedback.
/// Uses statistical pattern analysis with natural-language insight generation.
/// </summary>
public class AiInsightService : IAiInsightService
{
    private const double CriticalThreshold = 60;
    private const double WarningThreshold = 75;

    private readonly IAttendanceRepository _attendance;
    private readonly IEnrollmentRepository _enrollments;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IMarkRepository _marks;
    private readonly ILogger<AiInsightService> _logger;

    public AiInsightService(
        IAttendanceRepository attendance,
        IEnrollmentRepository enrollments,
        IClassRepository classes,
        IUserRepository users,
        IMarkRepository marks,
        ILogger<AiInsightService> logger)
    {
        _attendance = attendance;
        _enrollments = enrollments;
        _classes = classes;
        _users = users;
        _marks = marks;
        _logger = logger;
    }

    public async Task<AttendanceInsightResponse> GetAttendanceInsightsAsync(string? classId, string requesterId, string requesterRole)
    {
        _logger.LogInformation("Generating attendance insights for class {ClassId} by {User}", classId ?? "ALL", requesterId);

        var targetClasses = await ResolveClassesAsync(classId, requesterId, requesterRole);
        var atRisk = new List<StudentAttendanceRiskDto>();
        var allPercents = new List<double>();

        foreach (var cls in targetClasses)
        {
            var enrollments = await _enrollments.GetByClassAsync(cls.Id);
            var attendanceRecords = await _attendance.GetByClassAsync(cls.Id);

            foreach (var enrollment in enrollments)
            {
                var studentRecords = attendanceRecords
                    .Where(a => a.StudentId == enrollment.StudentId)
                    .OrderBy(a => a.Date)
                    .ToList();

                if (studentRecords.Count == 0) continue;

                var present = studentRecords.Count(r => r.IsPresent);
                var total = studentRecords.Count;
                var percent = total == 0 ? 100 : Math.Round(present * 100.0 / total, 1);
                allPercents.Add(percent);

                // Detect declining trend: last 3 vs previous sessions
                var recentTrend = DetectDecliningTrend(studentRecords);

                if (percent < WarningThreshold || recentTrend)
                {
                    var student = await _users.GetByIdAsync(enrollment.StudentId);
                    var risk = percent < CriticalThreshold || (recentTrend && percent < WarningThreshold)
                        ? "Critical"
                        : "Warning";

                    atRisk.Add(new StudentAttendanceRiskDto
                    {
                        StudentId = enrollment.StudentId,
                        StudentName = student?.FullName ?? "Unknown",
                        ClassId = cls.Id,
                        ClassName = cls.Name,
                        AttendancePercent = percent,
                        TotalSessions = total,
                        PresentCount = present,
                        AbsentCount = total - present,
                        RiskLevel = risk,
                        Warning = BuildWarning(percent, recentTrend, student?.FullName ?? "Student"),
                        Suggestion = BuildSuggestion(percent, recentTrend)
                    });
                }
            }
        }

        atRisk = atRisk.OrderBy(r => r.AttendancePercent).ToList();
        var avg = allPercents.Count == 0 ? 100 : Math.Round(allPercents.Average(), 1);

        return new AttendanceInsightResponse
        {
            Summary = BuildSummary(avg, atRisk.Count, allPercents.Count),
            ClassAverageAttendancePercent = avg,
            TotalStudentsAnalyzed = allPercents.Count,
            AtRiskCount = atRisk.Count,
            AtRiskStudents = atRisk,
            Suggestions = BuildClassSuggestions(avg, atRisk),
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<MarksFeedbackResponse> GenerateMarksFeedbackAsync(string markId, string requesterId, string requesterRole)
    {
        var mark = await _marks.GetByIdAsync(markId)
            ?? throw new KeyNotFoundException("Mark not found.");

        if (requesterRole == UserRoles.Student && mark.StudentId != requesterId)
            throw new UnauthorizedAccessException("You can only view feedback for your own marks.");

        var student = await _users.GetByIdAsync(mark.StudentId);
        var percentage = mark.MaxScore == 0 ? 0 : Math.Round(mark.Score / mark.MaxScore * 100, 2);
        var level = percentage switch
        {
            >= 90 => "Excellent",
            >= 75 => "Good",
            >= 60 => "Average",
            >= 40 => "Needs Improvement",
            _ => "At Risk"
        };

        var feedback = BuildSmartFeedback(student?.FullName ?? "Student", mark.ExamName, (double)percentage, level);

        mark.Feedback = feedback;
        await _marks.UpdateAsync(mark);

        return new MarksFeedbackResponse
        {
            MarkId = mark.Id,
            StudentName = student?.FullName ?? "Unknown",
            ExamName = mark.ExamName,
            Percentage = percentage,
            Feedback = feedback,
            PerformanceLevel = level
        };
    }

    private async Task<List<Models.ClassEntity>> ResolveClassesAsync(string? classId, string requesterId, string requesterRole)
    {
        if (!string.IsNullOrWhiteSpace(classId))
        {
            var cls = await _classes.GetByIdAsync(classId)
                ?? throw new ArgumentException("Class not found.");

            if (requesterRole == UserRoles.Teacher && cls.TeacherId != requesterId)
                throw new UnauthorizedAccessException("You can only analyze your own classes.");

            return [cls];
        }

        if (requesterRole == UserRoles.Teacher)
            return (await _classes.GetByTeacherAsync(requesterId)).ToList();

        if (requesterRole == UserRoles.Admin)
        {
            var (items, _) = await _classes.GetPagedAsync(1, 100);
            return items.ToList();
        }

        throw new UnauthorizedAccessException("Insufficient permissions for attendance insights.");
    }

    private static bool DetectDecliningTrend(List<Models.Attendance> records)
    {
        if (records.Count < 4) return false;

        var recent = records.TakeLast(3).ToList();
        var earlier = records.SkipLast(3).ToList();

        var recentRate = recent.Count(r => r.IsPresent) / (double)recent.Count;
        var earlierRate = earlier.Count(r => r.IsPresent) / (double)earlier.Count;

        return earlierRate - recentRate >= 0.3;
    }

    private static string BuildWarning(double percent, bool declining, string name)
    {
        if (percent < CriticalThreshold)
            return $"{name} is at critical risk with only {percent}% attendance.";
        if (declining)
            return $"{name} shows a declining attendance trend (currently {percent}%).";
        return $"{name} is below the recommended attendance threshold ({percent}%).";
    }

    private static string BuildSuggestion(double percent, bool declining)
    {
        if (percent < CriticalThreshold)
            return "Schedule a parent/guardian meeting and create an attendance improvement plan.";
        if (declining)
            return "Send an early warning notice and check for personal or academic blockers.";
        return "Monitor weekly and offer flexible catch-up sessions for missed classes.";
    }

    private static string BuildSummary(double avg, int atRiskCount, int total)
    {
        if (total == 0)
            return "No attendance data available yet. Mark attendance to unlock AI insights.";

        if (atRiskCount == 0)
            return $"Attendance looks healthy across {total} student(s). Class average is {avg}%.";

        return $"AI analysis flagged {atRiskCount} of {total} student(s) with low or declining attendance. Class average: {avg}%.";
    }

    private static List<string> BuildClassSuggestions(double avg, List<StudentAttendanceRiskDto> atRisk)
    {
        var suggestions = new List<string>();

        if (avg < WarningThreshold)
            suggestions.Add("Overall class attendance is below target — review timetable conflicts and engagement methods.");

        if (atRisk.Any(r => r.RiskLevel == "Critical"))
            suggestions.Add("Prioritize outreach for Critical-risk students within the next 48 hours.");

        if (atRisk.Count > 0)
            suggestions.Add("Share personalized reminders and consider short makeup sessions for frequently absent students.");

        if (suggestions.Count == 0)
            suggestions.Add("Maintain current practices and continue weekly attendance reviews.");

        return suggestions;
    }

    private static string BuildSmartFeedback(string name, string exam, double percentage, string level)
    {
        return level switch
        {
            "Excellent" => $"Great work, {name}! You scored {percentage}% on {exam}. Your mastery is strong — try peer tutoring or advanced problems to stay sharp.",
            "Good" => $"{name}, solid performance at {percentage}% on {exam}. Review missed questions carefully; you're close to the top band.",
            "Average" => $"{name} scored {percentage}% on {exam}. Focused revision on weak topics and short daily practice will lift results quickly.",
            "Needs Improvement" => $"{name}'s {percentage}% on {exam} needs attention. Recommend guided study sessions and checkpoint quizzes before the next assessment.",
            _ => $"{name} is at academic risk with {percentage}% on {exam}. Immediate remedial support and progress tracking are strongly recommended."
        };
    }
}
