using System.Security.Claims;

namespace RmConnect.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    /// <summary>The logged-in user's id, read from the auth cookie.</summary>
    public static Guid GetUserId(this ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
