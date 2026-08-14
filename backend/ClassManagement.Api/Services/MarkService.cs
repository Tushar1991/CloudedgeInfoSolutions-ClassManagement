using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Marks;
using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

public class MarkService : IMarkService
{
    private readonly IMarkRepository _marks;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ILogger<MarkService> _logger;

    public MarkService(
        IMarkRepository marks,
        IClassRepository classes,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        ILogger<MarkService> logger)
    {
        _marks = marks;
        _classes = classes;
        _users = users;
        _enrollments = enrollments;
        _logger = logger;
    }

    public async Task<MarkDto> CreateAsync(CreateMarkRequest request, string recordedBy)
    {
        if (request.Score > request.MaxScore)
            throw new ArgumentException("Score cannot exceed max score.");

        var classEntity = await _classes.GetByIdAsync(request.ClassId)
            ?? throw new ArgumentException("Class not found.");

        await EnsureCanManageClassAsync(classEntity, recordedBy);

        var enrollment = await _enrollments.GetAsync(request.ClassId, request.StudentId);
        if (enrollment is null)
            throw new InvalidOperationException("Student is not enrolled in this class.");

        var percentage = request.MaxScore == 0 ? 0 : (double)(request.Score / request.MaxScore * 100);
        var mark = new Mark
        {
            ClassId = request.ClassId,
            StudentId = request.StudentId,
            ExamName = request.ExamName.Trim(),
            Score = request.Score,
            MaxScore = request.MaxScore,
            RecordedBy = recordedBy,
            RecordedAt = DateTime.UtcNow,
            Feedback = GenerateBasicFeedback(percentage)
        };

        await _marks.CreateAsync(mark);
        _logger.LogInformation("Recorded mark for student {StudentId} exam {Exam}", request.StudentId, request.ExamName);
        return await MapAsync(mark);
    }

    public async Task<PagedResult<MarkDto>> GetPagedAsync(
        int page, int pageSize, string? classId = null, string? studentId = null,
        string? requesterId = null, string? requesterRole = null)
    {
        if (requesterRole == UserRoles.Student)
            studentId = requesterId;

        if (requesterRole == UserRoles.Teacher && requesterId is not null && !string.IsNullOrWhiteSpace(classId))
        {
            var cls = await _classes.GetByIdAsync(classId);
            if (cls is null || cls.TeacherId != requesterId)
                throw new UnauthorizedAccessException("You can only view marks for your classes.");
        }

        var (items, total) = await _marks.GetPagedAsync(page, pageSize, classId, studentId);
        var mapped = new List<MarkDto>();
        foreach (var item in items)
            mapped.Add(await MapAsync(item));

        return new PagedResult<MarkDto>
        {
            Items = mapped,
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MarkDto?> UpdateAsync(string id, UpdateMarkRequest request)
    {
        if (request.Score > request.MaxScore)
            throw new ArgumentException("Score cannot exceed max score.");

        var mark = await _marks.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Mark not found.");

        mark.ExamName = request.ExamName.Trim();
        mark.Score = request.Score;
        mark.MaxScore = request.MaxScore;
        var percentage = request.MaxScore == 0 ? 0 : (double)(request.Score / request.MaxScore * 100);
        mark.Feedback = GenerateBasicFeedback(percentage);

        await _marks.UpdateAsync(mark);
        return await MapAsync(mark);
    }

    public async Task<bool> DeleteAsync(string id) => await _marks.DeleteAsync(id);

    internal static string GenerateBasicFeedback(double percentage) => percentage switch
    {
        >= 90 => "Outstanding performance. Keep challenging yourself with advanced topics.",
        >= 75 => "Strong result. Focus on refining weaker areas to reach excellence.",
        >= 60 => "Satisfactory performance. Consistent practice will improve your score.",
        >= 40 => "Below expected level. Schedule extra revision sessions and seek help early.",
        _ => "Critical concern. Immediate academic support and attendance review recommended."
    };

    private async Task EnsureCanManageClassAsync(ClassEntity classEntity, string userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user is null) throw new UnauthorizedAccessException("Invalid user.");
        if (user.Role == UserRoles.Admin) return;
        if (user.Role == UserRoles.Teacher && classEntity.TeacherId == userId) return;
        throw new UnauthorizedAccessException("You can only manage marks for your own classes.");
    }

    private async Task<MarkDto> MapAsync(Mark m)
    {
        var classEntity = await _classes.GetByIdAsync(m.ClassId);
        var student = await _users.GetByIdAsync(m.StudentId);

        return new MarkDto
        {
            Id = m.Id,
            ClassId = m.ClassId,
            ClassName = classEntity?.Name ?? "Unknown",
            StudentId = m.StudentId,
            StudentName = student?.FullName ?? "Unknown",
            ExamName = m.ExamName,
            Score = m.Score,
            MaxScore = m.MaxScore,
            Feedback = m.Feedback,
            RecordedAt = m.RecordedAt
        };
    }
}
