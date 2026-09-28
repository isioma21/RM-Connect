using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RmConnect.Application.Common.Exceptions;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Users;

namespace RmConnect.Application.Sessions;

public class SessionService(IAppDbContext db, IOptions<SessionOptions> options, ILogger<SessionService> logger)
{
    public async Task<Guid> StartAsync(Guid userId, string ipAddress, string device, CancellationToken ct)
    {
        var user = await db.Users.FirstAsync(u => u.Id == userId, ct);
        var now = DateTime.UtcNow;

        var session = UserSession.Start(userId, ipAddress, device, now);
        user.RecordLogin(ipAddress, now);
        db.Sessions.Add(session);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Session {SessionId} started for user {UserId} from {IpAddress}", session.Id, userId, ipAddress);
        return session.Id;
    }

    public async Task<bool> IsActiveAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await db.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);
        if (session is null || session.IsRevoked)
            return false;

        session.MarkSeen(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<SessionResponse>> GetActiveAsync(Guid userId, Guid currentSessionId, CancellationToken ct)
    {
        var activeSince = DateTime.UtcNow.AddMinutes(-options.Value.SessionMinutes);
        var sessions = await db.Sessions
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.LastSeenAt > activeSince)
            .OrderByDescending(s => s.LastSeenAt)
            .ToListAsync(ct);

        return sessions.Select(s => SessionResponse.From(s, currentSessionId)).ToList();
    }

    public async Task RevokeAsync(Guid userId, Guid sessionId, CancellationToken ct)
    {
        var session = await db.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId, ct)
            ?? throw new NotFoundException("Session not found.");

        session.Revoke(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} signed out session {SessionId}", userId, sessionId);
    }

    public async Task RevokeOthersAsync(Guid userId, Guid currentSessionId, CancellationToken ct)
    {
        var otherSessions = await db.Sessions
            .Where(s => s.UserId == userId && s.Id != currentSessionId && s.RevokedAt == null)
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var session in otherSessions)
            session.Revoke(now);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} signed out {Count} other sessions", userId, otherSessions.Count);
    }
}
