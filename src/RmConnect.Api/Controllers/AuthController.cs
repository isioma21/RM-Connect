using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RmConnect.Api.Auth;
using RmConnect.Application.Auth;
using RmConnect.Application.Sessions;

namespace RmConnect.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService, SessionService sessionService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register/customer")]
    public async Task<ActionResult<UserResponse>> RegisterCustomer(RegisterCustomerRequest request, CancellationToken ct)
    {
        var user = await authService.RegisterCustomerAsync(request, ct);
        return CreatedAtAction(nameof(CurrentUser), user);
    }

    [HttpPost("register/manager")]
    public async Task<ActionResult<UserResponse>> RegisterManager(RegisterManagerRequest request, CancellationToken ct)
    {
        var user = await authService.RegisterManagerAsync(request, ct);
        return CreatedAtAction(nameof(CurrentUser), user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await authService.LoginAsync(request, ct);
        if (user is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var sessionId = await sessionService.StartAsync(user.Id, HttpContext.GetClientIp(), HttpContext.GetDevice(), ct);
        await SignInAsync(user, sessionId);
        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await sessionService.RevokeAsync(User.GetUserId(), User.GetSessionId(), ct);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        logger.LogInformation("User {UserId} logged out", User.GetUserId());
        return NoContent();
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<ActionResult<UserResponse>> CurrentUser(CancellationToken ct)
    {
        var user = await authService.GetUserAsync(User.GetUserId(), ct);
        return user is null ? Unauthorized() : Ok(user);
    }

    /// <summary>Issues the auth cookie holding the user's id, role and this device's session id.</summary>
    private Task SignInAsync(UserResponse user, Guid sessionId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimsPrincipalExtensions.SessionIdClaim, sessionId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
