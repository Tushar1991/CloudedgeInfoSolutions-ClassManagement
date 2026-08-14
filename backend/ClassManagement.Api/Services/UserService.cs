using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Users;
using ClassManagement.Api.Models;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Repositories.Interfaces;
using ClassManagement.Api.Services.Interfaces;

namespace ClassManagement.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository users, ILogger<UserService> logger)
    {
        _users = users;
        _logger = logger;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        if (!UserRoles.IsValid(request.Role))
            throw new ArgumentException($"Invalid role. Allowed: {string.Join(", ", UserRoles.All)}");

        if (await _users.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            Role = UserRoles.All.First(r => r.Equals(request.Role, StringComparison.OrdinalIgnoreCase)),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _users.CreateAsync(user);
        _logger.LogInformation("Created user {Email} with role {Role}", user.Email, user.Role);
        return Map(user);
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize, string? role = null)
    {
        var (items, total) = await _users.GetPagedAsync(page, pageSize, role);
        return new PagedResult<UserDto>
        {
            Items = items.Select(Map).ToList(),
            TotalCount = (int)total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _users.GetByIdAsync(id);
        return user is null ? null : Map(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetByRoleAsync(string role)
    {
        var users = await _users.GetByRoleAsync(role);
        return users.Select(Map).ToList();
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserRequest request)
    {
        if (!UserRoles.IsValid(request.Role))
            throw new ArgumentException($"Invalid role. Allowed: {string.Join(", ", UserRoles.All)}");

        var user = await _users.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found.");

        user.FullName = request.FullName.Trim();
        user.Role = UserRoles.All.First(r => r.Equals(request.Role, StringComparison.OrdinalIgnoreCase));
        user.IsActive = request.IsActive;

        await _users.UpdateAsync(user);
        return Map(user);
    }

    public async Task<bool> DeleteAsync(string id) => await _users.DeleteAsync(id);

    private static UserDto Map(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        FullName = u.FullName,
        Role = u.Role,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
