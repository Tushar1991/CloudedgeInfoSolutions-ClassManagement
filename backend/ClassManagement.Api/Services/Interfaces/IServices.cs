using ClassManagement.Api.DTOs.Ai;
using ClassManagement.Api.DTOs.Attendance;
using ClassManagement.Api.DTOs.Auth;
using ClassManagement.Api.DTOs.Classes;
using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Enrollments;
using ClassManagement.Api.DTOs.Marks;
using ClassManagement.Api.DTOs.Users;
using ClassManagement.Api.Models;

namespace ClassManagement.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    string GenerateToken(User user);
}

public interface IUserService
{
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize, string? role = null);
    Task<UserDto?> GetByIdAsync(string id);
    Task<IReadOnlyList<UserDto>> GetByRoleAsync(string role);
    Task<UserDto?> UpdateAsync(string id, UpdateUserRequest request);
    Task<bool> DeleteAsync(string id);
}

public interface IClassService
{
    Task<ClassDto> CreateAsync(CreateClassRequest request);
    Task<PagedResult<ClassDto>> GetPagedAsync(int page, int pageSize, string? teacherId = null, string? requesterId = null, string? requesterRole = null);
    Task<ClassDto?> GetByIdAsync(string id);
    Task<ClassDto?> UpdateAsync(string id, UpdateClassRequest request);
    Task<bool> DeleteAsync(string id);
}

public interface IEnrollmentService
{
    Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request);
    Task<PagedResult<EnrollmentDto>> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null, string? requesterId = null, string? requesterRole = null);
    Task<bool> DeleteAsync(string id);
}

public interface IAttendanceService
{
    Task<AttendanceDto> MarkAsync(MarkAttendanceRequest request, string markedBy);
    Task<IReadOnlyList<AttendanceDto>> BulkMarkAsync(BulkAttendanceRequest request, string markedBy);
    Task<PagedResult<AttendanceDto>> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null, string? requesterId = null, string? requesterRole = null);
}

public interface IMarkService
{
    Task<MarkDto> CreateAsync(CreateMarkRequest request, string recordedBy);
    Task<PagedResult<MarkDto>> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null, string? requesterId = null, string? requesterRole = null);
    Task<MarkDto?> UpdateAsync(string id, UpdateMarkRequest request);
    Task<bool> DeleteAsync(string id);
}

public interface IAiInsightService
{
    Task<AttendanceInsightResponse> GetAttendanceInsightsAsync(string? classId, string requesterId, string requesterRole);
    Task<MarksFeedbackResponse> GenerateMarksFeedbackAsync(string markId, string requesterId, string requesterRole);
}
