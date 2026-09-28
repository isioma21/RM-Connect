using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RmConnect.Api.Auth;
using RmConnect.Application.Sessions;

namespace RmConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions")]
public class SessionsController(SessionService sessionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SessionResponse>>> GetMine(CancellationToken ct)
    {
        return await sessionService.GetActiveAsync(User.GetUserId(), User.GetSessionId(), ct);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> SignOutDevice(Guid id, CancellationToken ct)
    {
        await sessionService.RevokeAsync(User.GetUserId(), id, ct);

        if (id == User.GetSessionId())
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }

    [HttpPost("revoke-others")]
    public async Task<IActionResult> SignOutOtherDevices(CancellationToken ct)
    {
        await sessionService.RevokeOthersAsync(User.GetUserId(), User.GetSessionId(), ct);
        return NoContent();
    }
}
