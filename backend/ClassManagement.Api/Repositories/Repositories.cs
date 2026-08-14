using ClassManagement.Api.Data;
using ClassManagement.Api.Models;
using ClassManagement.Api.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClassManagement.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoDbContext context) => _collection = context.Users;

    public async Task<User?> GetByIdAsync(string id) =>
        await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await _collection.Find(u => u.Email == email.ToLowerInvariant()).FirstOrDefaultAsync();

    public async Task<(IReadOnlyList<User> Items, long Total)> GetPagedAsync(int page, int pageSize, string? role = null)
    {
        var filter = string.IsNullOrWhiteSpace(role)
            ? Builders<User>.Filter.Empty
            : Builders<User>.Filter.Eq(u => u.Role, role);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(string role) =>
        await _collection.Find(u => u.Role == role && u.IsActive).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        user.Email = user.Email.ToLowerInvariant();
        await _collection.InsertOneAsync(user);
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var result = await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _collection.Find(u => u.Email == email.ToLowerInvariant()).AnyAsync();
}

public class ClassRepository : IClassRepository
{
    private readonly IMongoCollection<ClassEntity> _collection;

    public ClassRepository(MongoDbContext context) => _collection = context.Classes;

    public async Task<ClassEntity?> GetByIdAsync(string id) =>
        await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<(IReadOnlyList<ClassEntity> Items, long Total)> GetPagedAsync(int page, int pageSize, string? teacherId = null)
    {
        var filter = string.IsNullOrWhiteSpace(teacherId)
            ? Builders<ClassEntity>.Filter.Empty
            : Builders<ClassEntity>.Filter.Eq(c => c.TeacherId, teacherId);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<ClassEntity>> GetByTeacherAsync(string teacherId) =>
        await _collection.Find(c => c.TeacherId == teacherId).ToListAsync();

    public async Task<ClassEntity> CreateAsync(ClassEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<bool> UpdateAsync(ClassEntity entity)
    {
        var result = await _collection.ReplaceOneAsync(c => c.Id == entity.Id, entity);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }
}

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly IMongoCollection<Enrollment> _collection;

    public EnrollmentRepository(MongoDbContext context) => _collection = context.Enrollments;

    public async Task<Enrollment?> GetByIdAsync(string id) =>
        await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

    public async Task<Enrollment?> GetAsync(string classId, string studentId) =>
        await _collection.Find(e => e.ClassId == classId && e.StudentId == studentId).FirstOrDefaultAsync();

    public async Task<(IReadOnlyList<Enrollment> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null)
    {
        var filter = Builders<Enrollment>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(classId))
            filter &= Builders<Enrollment>.Filter.Eq(e => e.ClassId, classId);
        if (!string.IsNullOrWhiteSpace(studentId))
            filter &= Builders<Enrollment>.Filter.Eq(e => e.StudentId, studentId);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(e => e.EnrolledAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByClassAsync(string classId) =>
        await _collection.Find(e => e.ClassId == classId).ToListAsync();

    public async Task<IReadOnlyList<Enrollment>> GetByStudentAsync(string studentId) =>
        await _collection.Find(e => e.StudentId == studentId).ToListAsync();

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        await _collection.InsertOneAsync(enrollment);
        return enrollment;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(e => e.Id == id);
        return result.DeletedCount > 0;
    }
}

public class AttendanceRepository : IAttendanceRepository
{
    private readonly IMongoCollection<Attendance> _collection;

    public AttendanceRepository(MongoDbContext context) => _collection = context.Attendance;

    public async Task<Attendance?> GetByIdAsync(string id) =>
        await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();

    public async Task<(IReadOnlyList<Attendance> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null)
    {
        var filter = Builders<Attendance>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(classId))
            filter &= Builders<Attendance>.Filter.Eq(a => a.ClassId, classId);
        if (!string.IsNullOrWhiteSpace(studentId))
            filter &= Builders<Attendance>.Filter.Eq(a => a.StudentId, studentId);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(a => a.Date)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<Attendance>> GetByClassAsync(string classId) =>
        await _collection.Find(a => a.ClassId == classId).ToListAsync();

    public async Task<IReadOnlyList<Attendance>> GetByStudentAsync(string studentId) =>
        await _collection.Find(a => a.StudentId == studentId).ToListAsync();

    public async Task<Attendance?> GetByKeyAsync(string classId, string studentId, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
        return await _collection.Find(a =>
            a.ClassId == classId &&
            a.StudentId == studentId &&
            a.Date >= dayStart &&
            a.Date < dayEnd).FirstOrDefaultAsync();
    }

    public async Task<Attendance> UpsertAsync(Attendance attendance)
    {
        var existing = await GetByKeyAsync(attendance.ClassId, attendance.StudentId, attendance.Date);
        if (existing is null)
        {
            await _collection.InsertOneAsync(attendance);
            return attendance;
        }

        existing.IsPresent = attendance.IsPresent;
        existing.Notes = attendance.Notes;
        existing.MarkedBy = attendance.MarkedBy;
        await _collection.ReplaceOneAsync(a => a.Id == existing.Id, existing);
        return existing;
    }

    public async Task InsertManyAsync(IEnumerable<Attendance> records) =>
        await _collection.InsertManyAsync(records);
}

public class MarkRepository : IMarkRepository
{
    private readonly IMongoCollection<Mark> _collection;

    public MarkRepository(MongoDbContext context) => _collection = context.Marks;

    public async Task<Mark?> GetByIdAsync(string id) =>
        await _collection.Find(m => m.Id == id).FirstOrDefaultAsync();

    public async Task<(IReadOnlyList<Mark> Items, long Total)> GetPagedAsync(int page, int pageSize, string? classId = null, string? studentId = null)
    {
        var filter = Builders<Mark>.Filter.Empty;
        if (!string.IsNullOrWhiteSpace(classId))
            filter &= Builders<Mark>.Filter.Eq(m => m.ClassId, classId);
        if (!string.IsNullOrWhiteSpace(studentId))
            filter &= Builders<Mark>.Filter.Eq(m => m.StudentId, studentId);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(m => m.RecordedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<Mark>> GetByStudentAsync(string studentId) =>
        await _collection.Find(m => m.StudentId == studentId).ToListAsync();

    public async Task<Mark> CreateAsync(Mark mark)
    {
        await _collection.InsertOneAsync(mark);
        return mark;
    }

    public async Task<bool> UpdateAsync(Mark mark)
    {
        var result = await _collection.ReplaceOneAsync(m => m.Id == mark.Id, mark);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(m => m.Id == id);
        return result.DeletedCount > 0;
    }
}
