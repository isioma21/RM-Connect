namespace RmConnect.Domain.Users;

/// <summary>One signed-in device. Revoking it signs that device out on its next request.</summary>
public class UserSession
{
    private const int MaxDeviceLength = 200;

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string IpAddress { get; private set; } = default!;
    public string Device { get; private set; } = default!;
    public DateTime SignedInAt { get; private set; }
    public DateTime LastSeenAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private UserSession() { }

    public static UserSession Start(Guid userId, string ipAddress, string device, DateTime now)
    {
        return new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IpAddress = ipAddress,
            Device = device.Length > MaxDeviceLength ? device[..MaxDeviceLength] : device,
            SignedInAt = now,
            LastSeenAt = now
        };
    }

    public bool IsRevoked => RevokedAt is not null;

    public void MarkSeen(DateTime now) => LastSeenAt = now;

    public void Revoke(DateTime now) => RevokedAt ??= now;
}
