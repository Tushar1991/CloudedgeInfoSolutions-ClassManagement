using System.Security.Claims;

namespace ClassManagement.Api.Helpers;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User id claim missing.");

    public static string GetUserRole(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role)
        ?? throw new UnauthorizedAccessException("Role claim missing.");
}
