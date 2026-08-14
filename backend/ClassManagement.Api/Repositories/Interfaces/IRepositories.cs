using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.Models;
using MongoDB.Driver;

namespace ClassManagement.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<(IReadOnlyList<User> Items, long Total)> GetPagedAsync(int page, int pageSize, string? role = null);
    Task<IReadOnlyList<User>> GetByRoleAsync(string role);
    Task<User> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(string id);
    Task<bool> EmailExistsAsync(string email);
}

public interface IClassRepository
{
    Task<ClassEntity?> GetByIdAsync(string id);
    Task<(IReadOnlyList<ClassEntity> Items, long Total)> GetPagedAsync(int page, int pageSize, string? teacherId = null);
    Task<IReadOnlyList<ClassEntity>> GetByTeacherAsync(string teacherId);
    Task<ClassEntity> CreateAsync(ClassEntity entity);
    Task<bool> UpdateAsync(ClassEntity entity);
    Task<bool> DeleteAsync(string id);
}

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(string id);
    Task<Enrollment?> GetAsync(string classId, string studentId);
    Task<(IReadOnlyList<Enrollment> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null);
    Task<IReadOnlyList<Enrollment>> GetByClassAsync(string classId);
    Task<IReadOnlyList<Enrollment>> GetByStudentAsync(string studentId);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(string id);
}

public interface IAttendanceRepository
{
    Task<Attendance?> GetByIdAsync(string id);
    Task<(IReadOnlyList<Attendance> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null);
    Task<IReadOnlyList<Attendance>> GetByClassAsync(string classId);
    Task<IReadOnlyList<Attendance>> GetByStudentAsync(string studentId);
    Task<Attendance?> GetByKeyAsync(string classId, string studentId, DateTime date);
    Task<Attendance> UpsertAsync(Attendance attendance);
    Task InsertManyAsync(IEnumerable<Attendance> records);
}

public interface IMarkRepository
{
    Task<Mark?> GetByIdAsync(string id);
    Task<(IReadOnlyList<Mark> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null);
    Task<IReadOnlyList<Mark>> GetByStudentAsync(string studentId);
    Task<Mark> CreateAsync(Mark mark);
    Task<bool> UpdateAsync(Mark mark);
    Task<bool> DeleteAsync(string id);
}
