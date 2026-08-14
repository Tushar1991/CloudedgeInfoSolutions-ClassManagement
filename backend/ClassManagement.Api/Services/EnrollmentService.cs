using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Enrollments;
using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollments;
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IEnrollmentRepository enrollments,
        IClassRepository classes,
        IUserRepository users,
        ILogger<EnrollmentService> logger)
    {
        _enrollments = enrollments;
        _classes = classes;
        _users = users;
        _logger = logger;
    }

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request)
    {
        var classEntity = await _classes.GetByIdAsync(request.ClassId)
            ?? throw new ArgumentException("Class not found.");

        var student = await _users.GetByIdAsync(request.StudentId)
            ?? throw new ArgumentException("Student not found.");

        if (!student.Role.Equals(UserRoles.Student, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Selected user must have Student role.");

        var existing = await _enrollments.GetAsync(request.ClassId, request.StudentId);
        if (existing is not null)
            throw new InvalidOperationException("Student is already enrolled in this class.");

        var enrollment = new Enrollment
        {
            ClassId = request.ClassId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };

        await _enrollments.CreateAsync(enrollment);
        _logger.LogInformation("Enrolled student {StudentId} in class {ClassId}", request.StudentId, request.ClassId);
        return await MapAsync(enrollment, classEntity, student);
    }

    public async Task<PagedResult<EnrollmentDto>> GetPagedAsync(
        int page, int pageSize, string? classId = null, string? studentId = null,
        string? requesterId = null, string? requesterRole = null)
    {
        if (requesterRole == UserRoles.Student)
            studentId = requesterId;

        if (requesterRole == UserRoles.Teacher && requesterId is not null && string.IsNullOrWhiteSpace(classId))
        {
            var teacherClasses = await _classes.GetByTeacherAsync(requesterId);
            var all = new List<EnrollmentDto>();
            foreach (var tc in teacherClasses)
            {
                var classEnrollments = await _enrollments.GetByClassAsync(tc.Id);
                foreach (var e in classEnrollments)
                    all.Add(await MapAsync(e));
            }

            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<EnrollmentDto>
            {
                Items = paged,
                TotalCount = all.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        if (requesterRole == UserRoles.Teacher && !string.IsNullOrWhiteSpace(classId))
        {
            var cls = await _classes.GetByIdAsync(classId);
            if (cls is null || cls.TeacherId != requesterId)
                throw new UnauthorizedAccessException("You can only view enrollments for your classes.");
        }

        var (items, total) = await _enrollments.GetPagedAsync(page, pageSize, classId, studentId);
        var mapped = new List<EnrollmentDto>();
        foreach (var item in items)
            mapped.Add(await MapAsync(item));

        return new PagedResult<EnrollmentDto>
        {
            Items = mapped,
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> DeleteAsync(string id) => await _enrollments.DeleteAsync(id);

    private async Task<EnrollmentDto> MapAsync(Enrollment e, ClassEntity? classEntity = null, User? student = null)
    {
        classEntity ??= await _classes.GetByIdAsync(e.ClassId);
        student ??= await _users.GetByIdAsync(e.StudentId);

        return new EnrollmentDto
        {
            Id = e.Id,
            ClassId = e.ClassId,
            ClassName = classEntity?.Name ?? "Unknown",
            StudentId = e.StudentId,
            StudentName = student?.FullName ?? "Unknown",
            EnrolledAt = e.EnrolledAt
        };
    }
}
