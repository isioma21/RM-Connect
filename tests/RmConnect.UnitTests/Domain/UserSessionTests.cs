using RmConnect.Domain.Users;

namespace RmConnect.UnitTests.Domain;

public class UserSessionTests
{
    [Fact]
    public void New_session_is_active_and_seen_now()
    {
        var session = UserSession.Start(Guid.NewGuid(), "10.0.0.1", "Chrome on Windows", TestUsers.Now);

        Assert.False(session.IsRevoked);
        Assert.Equal(TestUsers.Now, session.LastSeenAt);
    }

    [Fact]
    public void Revoked_session_keeps_the_first_revoke_time()
    {
        var session = UserSession.Start(Guid.NewGuid(), "10.0.0.1", "Chrome on Windows", TestUsers.Now);

        session.Revoke(TestUsers.Now);
        session.Revoke(TestUsers.Now.AddHours(1));

        Assert.True(session.IsRevoked);
        Assert.Equal(TestUsers.Now, session.RevokedAt);
    }

    [Fact]
    public void Very_long_device_names_are_cut_to_200_characters()
    {
        var session = UserSession.Start(Guid.NewGuid(), "10.0.0.1", new string('x', 500), TestUsers.Now);

        Assert.Equal(200, session.Device.Length);
    }
}
