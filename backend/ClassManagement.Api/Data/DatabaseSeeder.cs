using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using MongoDB.Driver;

namespace ClassManagement.Api.Data;

public class DatabaseSeeder
{
    private readonly MongoDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(MongoDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.EnsureIndexesAsync();

            var adminExists = await _context.Users
                .Find(u => u.Email == "admin@classmgmt.local")
                .AnyAsync();

            if (adminExists)
            {
                _logger.LogInformation("Database already seeded.");
                return;
            }

            var admin = new User
            {
                Email = "admin@classmgmt.local",
                FullName = "System Admin",
                Role = UserRoles.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var teacher = new User
            {
                Email = "teacher@classmgmt.local",
                FullName = "Demo Teacher",
                Role = UserRoles.Teacher,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var student = new User
            {
                Email = "student@classmgmt.local",
                FullName = "Demo Student",
                Role = UserRoles.Student,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.InsertManyAsync([admin, teacher, student]);
            _logger.LogInformation("Seeded default Admin, Teacher, and Student accounts.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Database seeding skipped because MongoDB is unavailable.");
        }
    }
}
