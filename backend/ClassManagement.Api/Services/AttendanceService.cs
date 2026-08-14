using ClassManagement.Api.DTOs.Attendance;
using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendance;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ILogger<AttendanceService> _logger;

    public AttendanceService(
        IAttendanceRepository attendance,
        IClassRepository classes,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        ILogger<AttendanceService> logger)
    {
        _attendance = attendance;
        _classes = classes;
        _users = users;
        _enrollments = enrollments;
        _logger = logger;
    }

    public async Task<AttendanceDto> MarkAsync(MarkAttendanceRequest request, string markedBy)
    {
        await ValidateClassAndEnrollmentAsync(request.ClassId, request.StudentId, markedBy);

        var record = new Attendance
        {
            ClassId = request.ClassId,
            StudentId = request.StudentId,
            Date = request.Date.Date,
            IsPresent = request.IsPresent,
            Notes = request.Notes,
            MarkedBy = markedBy,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _attendance.UpsertAsync(record);
        _logger.LogInformation("Marked attendance for student {StudentId} in class {ClassId}", request.StudentId, request.ClassId);
        return await MapAsync(saved);
    }

    public async Task<IReadOnlyList<AttendanceDto>> BulkMarkAsync(BulkAttendanceRequest request, string markedBy)
    {
        var classEntity = await _classes.GetByIdAsync(request.ClassId)
            ?? throw new ArgumentException("Class not found.");

        await EnsureTeacherOwnsClassAsync(classEntity, markedBy);

        var results = new List<AttendanceDto>();
        foreach (var item in request.Students)
        {
            var enrollment = await _enrollments.GetAsync(request.ClassId, item.StudentId);
            if (enrollment is null) continue;

            var record = new Attendance
            {
                ClassId = request.ClassId,
                StudentId = item.StudentId,
                Date = request.Date.Date,
                IsPresent = item.IsPresent,
                Notes = item.Notes,
                MarkedBy = markedBy,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _attendance.UpsertAsync(record);
            results.Add(await MapAsync(saved));
        }

        return results;
    }

    public async Task<PagedResult<AttendanceDto>> GetPagedAsync(
        int page, int pageSize, string? classId = null, string? studentId = null,
        string? requesterId = null, string? requesterRole = null)
    {
        if (requesterRole == UserRoles.Student)
            studentId = requesterId;

        if (requesterRole == UserRoles.Teacher && requesterId is not null && !string.IsNullOrWhiteSpace(classId))
        {
            var cls = await _classes.GetByIdAsync(classId);
            if (cls is null || cls.TeacherId != requesterId)
                throw new UnauthorizedAccessException("You can only view attendance for your classes.");
        }

        var (items, total) = await _attendance.GetPagedAsync(page, pageSize, classId, studentId);
        var mapped = new List<AttendanceDto>();
        foreach (var item in items)
            mapped.Add(await MapAsync(item));

        return new PagedResult<AttendanceDto>
        {
            Items = mapped,
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    private async Task ValidateClassAndEnrollmentAsync(string classId, string studentId, string markedBy)
    {
        var classEntity = await _classes.GetByIdAsync(classId)
            ?? throw new ArgumentException("Class not found.");

        await EnsureTeacherOwnsClassAsync(classEntity, markedBy);

        var enrollment = await _enrollments.GetAsync(classId, studentId);
        if (enrollment is null)
            throw new InvalidOperationException("Student is not enrolled in this class.");
    }

    private async Task EnsureTeacherOwnsClassAsync(ClassEntity classEntity, string markedBy)
    {
        var marker = await _users.GetByIdAsync(markedBy);
        if (marker is null) throw new UnauthorizedAccessException("Invalid user.");

        if (marker.Role == UserRoles.Admin) return;

        if (marker.Role == UserRoles.Teacher && classEntity.TeacherId == markedBy) return;

        throw new UnauthorizedAccessException("You can only mark attendance for your own classes.");
    }

    private async Task<AttendanceDto> MapAsync(Attendance a)
    {
        var classEntity = await _classes.GetByIdAsync(a.ClassId);
        var student = await _users.GetByIdAsync(a.StudentId);

        return new AttendanceDto
        {
            Id = a.Id,
            ClassId = a.ClassId,
            ClassName = classEntity?.Name ?? "Unknown",
            StudentId = a.StudentId,
            StudentName = student?.FullName ?? "Unknown",
            Date = a.Date,
            IsPresent = a.IsPresent,
            Notes = a.Notes,
            CreatedAt = a.CreatedAt
        };
    }
}
