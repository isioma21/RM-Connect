using RmConnect.Domain.Users;

namespace RmConnect.Application.Sessions;

public record SessionResponse(Guid Id, string Device, string IpAddress, DateTime SignedInAt, DateTime LastSeenAt, bool IsCurrent)
{
    public static SessionResponse From(UserSession session, Guid currentSessionId) =>
        new(session.Id, session.Device, session.IpAddress, session.SignedInAt, session.LastSeenAt, session.Id == currentSessionId);
}
