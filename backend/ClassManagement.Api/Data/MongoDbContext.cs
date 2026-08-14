using ClassManagement.Api.Configuration;
using ClassManagement.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClassManagement.Api.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<ClassEntity> Classes => _database.GetCollection<ClassEntity>("Classes");
    public IMongoCollection<Enrollment> Enrollments => _database.GetCollection<Enrollment>("Enrollments");
    public IMongoCollection<Attendance> Attendance => _database.GetCollection<Attendance>("Attendance");
    public IMongoCollection<Mark> Marks => _database.GetCollection<Mark>("Marks");

    public async Task EnsureIndexesAsync()
    {
        await Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true }));

        await Enrollments.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                Builders<Enrollment>.IndexKeys
                    .Ascending(e => e.ClassId)
                    .Ascending(e => e.StudentId),
                new CreateIndexOptions { Unique = true }));

        await Attendance.Indexes.CreateOneAsync(
            new CreateIndexModel<Attendance>(
                Builders<Attendance>.IndexKeys
                    .Ascending(a => a.ClassId)
                    .Ascending(a => a.StudentId)
                    .Ascending(a => a.Date),
                new CreateIndexOptions { Unique = true }));
    }
}
