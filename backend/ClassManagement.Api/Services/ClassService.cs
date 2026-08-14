using ClassManagement.Api.DTOs.Classes;
using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

public class ClassService : IClassService
{
    private readonly IClassRepository _classes;
    private readonly IUserRepository _users;
    private readonly IEnrollmentRepository _enrollments;
    private readonly ILogger<ClassService> _logger;

    public ClassService(
        IClassRepository classes,
        IUserRepository users,
        IEnrollmentRepository enrollments,
        ILogger<ClassService> logger)
    {
        _classes = classes;
        _users = users;
        _enrollments = enrollments;
        _logger = logger;
    }

    public async Task<ClassDto> CreateAsync(CreateClassRequest request)
    {
        var teacher = await _users.GetByIdAsync(request.TeacherId)
            ?? throw new ArgumentException("Teacher not found.");

        if (!teacher.Role.Equals(UserRoles.Teacher, StringComparison.OrdinalIgnoreCase) &&
            !teacher.Role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Assigned user must be a Teacher.");

        var entity = new ClassEntity
        {
            Name = request.Name.Trim(),
            Subject = request.Subject.Trim(),
            TeacherId = request.TeacherId,
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _classes.CreateAsync(entity);
        _logger.LogInformation("Created class {ClassName}", entity.Name);
        return await MapAsync(entity);
    }

    public async Task<PagedResult<ClassDto>> GetPagedAsync(int page, int pageSize, string? teacherId = null, string? requesterId = null, string? requesterRole = null)
    {
        if (requesterRole == UserRoles.Teacher)
            teacherId = requesterId;

        if (requesterRole == UserRoles.Student && requesterId is not null)
        {
            var enrollments = await _enrollments.GetByStudentAsync(requesterId);
            var classIds = enrollments.Select(e => e.ClassId).ToHashSet();
            var all = new List<ClassDto>();
            foreach (var classId in classIds)
            {
                var c = await _classes.GetByIdAsync(classId);
                if (c is not null) all.Add(await MapAsync(c));
            }

            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<ClassDto>
            {
                Items = paged,
                TotalCount = all.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        var (items, total) = await _classes.GetPagedAsync(page, pageSize, teacherId);
        var mapped = new List<ClassDto>();
        foreach (var item in items)
            mapped.Add(await MapAsync(item));

        return new PagedResult<ClassDto>
        {
            Items = mapped,
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ClassDto?> GetByIdAsync(string id)
    {
        var entity = await _classes.GetByIdAsync(id);
        return entity is null ? null : await MapAsync(entity);
    }

    public async Task<ClassDto?> UpdateAsync(string id, UpdateClassRequest request)
    {
        var entity = await _classes.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Class not found.");

        var teacher = await _users.GetByIdAsync(request.TeacherId)
            ?? throw new ArgumentException("Teacher not found.");

        if (!teacher.Role.Equals(UserRoles.Teacher, StringComparison.OrdinalIgnoreCase) &&
            !teacher.Role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Assigned user must be a Teacher.");

        entity.Name = request.Name.Trim();
        entity.Subject = request.Subject.Trim();
        entity.TeacherId = request.TeacherId;
        entity.Description = request.Description?.Trim();

        await _classes.UpdateAsync(entity);
        return await MapAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id) => await _classes.DeleteAsync(id);

    private async Task<ClassDto> MapAsync(ClassEntity c)
    {
        var teacher = await _users.GetByIdAsync(c.TeacherId);
        return new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            Subject = c.Subject,
            TeacherId = c.TeacherId,
            TeacherName = teacher?.FullName ?? "Unknown",
            Description = c.Description,
            CreatedAt = c.CreatedAt
        };
    }
}
