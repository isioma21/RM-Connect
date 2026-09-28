using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RmConnect.Api.Auth;
using RmConnect.Application.Auth;
using RmConnect.Application.Relationships;
using RmConnect.Domain.Users;

namespace RmConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/relationships")]
public class RelationshipsController(RelationshipService relationshipService) : ControllerBase
{
    [HttpGet("managers")]
    public async Task<ActionResult<List<UserResponse>>> GetManagers(CancellationToken ct)
    {
        return await relationshipService.GetManagersAsync(ct);
    }

    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpPost]
    public async Task<ActionResult<RelationshipResponse>> RequestManager(RequestRelationshipRequest request, CancellationToken ct)
    {
        var relationship = await relationshipService.RequestAsync(User.GetUserId(), request.ManagerId, ct);
        return CreatedAtAction(nameof(GetCurrent), relationship);
    }

    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpGet("current")]
    public async Task<ActionResult<RelationshipResponse>> GetCurrent(CancellationToken ct)
    {
        var relationship = await relationshipService.GetCurrentAsync(User.GetUserId(), ct);
        return relationship is null ? NoContent() : Ok(relationship);
    }

    [Authorize(Roles = nameof(UserRole.RelationshipManager))]
    [HttpGet]
    public async Task<ActionResult<List<RelationshipResponse>>> GetForManager(CancellationToken ct)
    {
        return await relationshipService.GetForManagerAsync(User.GetUserId(), ct);
    }

    [Authorize(Roles = nameof(UserRole.RelationshipManager))]
    [HttpPost("{id:guid}/accept")]
    public async Task<ActionResult<RelationshipResponse>> Accept(Guid id, CancellationToken ct)
    {
        return await relationshipService.AcceptAsync(User.GetUserId(), id, ct);
    }

    [Authorize(Roles = nameof(UserRole.RelationshipManager))]
    [HttpPost("{id:guid}/decline")]
    public async Task<ActionResult<RelationshipResponse>> Decline(Guid id, CancellationToken ct)
    {
        return await relationshipService.DeclineAsync(User.GetUserId(), id, ct);
    }

    [HttpPost("{id:guid}/end")]
    public async Task<ActionResult<RelationshipResponse>> End(Guid id, CancellationToken ct)
    {
        return await relationshipService.EndAsync(User.GetUserId(), id, ct);
    }
}
