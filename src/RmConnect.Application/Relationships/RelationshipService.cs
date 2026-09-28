using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RmConnect.Application.Auth;
using RmConnect.Application.Common.Exceptions;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Relationships;
using RmConnect.Domain.Users;

namespace RmConnect.Application.Relationships;

public class RelationshipService(IAppDbContext db, ILogger<RelationshipService> logger)
{
    public async Task<List<UserResponse>> GetManagersAsync(CancellationToken ct)
    {
        var managers = await db.Users
            .Where(u => u.Role == UserRole.RelationshipManager)
            .OrderBy(u => u.FirstName)
            .ToListAsync(ct);

        return managers.Select(UserResponse.From).ToList();
    }

    public async Task<RelationshipResponse> RequestAsync(Guid customerId, Guid managerId, CancellationToken ct)
    {
        if (await GetCurrentAsync(customerId, ct) is not null)
            throw new ConflictException("You already have a pending or active relationship manager.");

        var customer = await db.Users.FirstAsync(u => u.Id == customerId, ct);
        var manager = await db.Users.FirstOrDefaultAsync(u => u.Id == managerId, ct)
            ?? throw new NotFoundException("Relationship manager not found.");

        var relationship = Relationship.Request(customer, manager, DateTime.UtcNow);
        db.Relationships.Add(relationship);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Customer {CustomerId} requested manager {ManagerId}", customerId, managerId);
        return RelationshipResponse.From(relationship);
    }

    public async Task<RelationshipResponse?> GetCurrentAsync(Guid customerId, CancellationToken ct)
    {
        var relationship = await db.Relationships
            .Include(r => r.Customer)
            .Include(r => r.Manager)
            .FirstOrDefaultAsync(r => r.CustomerId == customerId &&
                (r.Status == RelationshipStatus.Pending || r.Status == RelationshipStatus.Active), ct);

        return relationship is null ? null : RelationshipResponse.From(relationship);
    }

    public async Task<List<RelationshipResponse>> GetForManagerAsync(Guid managerId, CancellationToken ct)
    {
        var relationships = await db.Relationships
            .Include(r => r.Customer)
            .Include(r => r.Manager)
            .Where(r => r.ManagerId == managerId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);

        return relationships.Select(RelationshipResponse.From).ToList();
    }

    public async Task<RelationshipResponse> AcceptAsync(Guid managerId, Guid relationshipId, CancellationToken ct)
    {
        var relationship = await FindAsync(relationshipId, managerId, ct);
        relationship.Accept(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Manager {ManagerId} accepted relationship {RelationshipId}", managerId, relationshipId);
        return RelationshipResponse.From(relationship);
    }

    public async Task<RelationshipResponse> DeclineAsync(Guid managerId, Guid relationshipId, CancellationToken ct)
    {
        var relationship = await FindAsync(relationshipId, managerId, ct);
        relationship.Decline(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Manager {ManagerId} declined relationship {RelationshipId}", managerId, relationshipId);
        return RelationshipResponse.From(relationship);
    }

    public async Task<RelationshipResponse> EndAsync(Guid userId, Guid relationshipId, CancellationToken ct)
    {
        var relationship = await FindAsync(relationshipId, userId, ct);
        relationship.End(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} ended relationship {RelationshipId}", userId, relationshipId);
        return RelationshipResponse.From(relationship);
    }

    private async Task<Relationship> FindAsync(Guid relationshipId, Guid userId, CancellationToken ct)
    {
        var relationship = await db.Relationships
            .Include(r => r.Customer)
            .Include(r => r.Manager)
            .FirstOrDefaultAsync(r => r.Id == relationshipId && (r.CustomerId == userId || r.ManagerId == userId), ct);

        return relationship ?? throw new NotFoundException("Relationship not found.");
    }
}
