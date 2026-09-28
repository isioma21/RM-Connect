using System.Security.Claims;

namespace RmConnect.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public const string SessionIdClaim = "session_id";

    /// <summary>The logged-in user's id, read from the auth cookie.</summary>
    public static Guid GetUserId(this ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>The id of this device's session, read from the auth cookie.</summary>
    public static Guid GetSessionId(this ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(SessionIdClaim)!);
}
