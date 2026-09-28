using RmConnect.Domain.Users;

namespace RmConnect.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Email_is_stored_trimmed_and_lowercase()
    {
        var user = User.RegisterCustomer("Chidi", "Nwosu", "  Chidi.Nwosu@Example.com ", "08031234567", "hash", TestUsers.Now);

        Assert.Equal("chidi.nwosu@example.com", user.Email);
    }

    [Fact]
    public void Customer_is_registered_with_phone_and_customer_role()
    {
        var user = TestUsers.Customer();

        Assert.Equal(UserRole.Customer, user.Role);
        Assert.Equal("08031234567", user.Phone);
        Assert.False(user.IsRelationshipManager);
    }

    [Fact]
    public void Manager_is_registered_with_branch_and_manager_role()
    {
        var user = TestUsers.Manager();

        Assert.Equal(UserRole.RelationshipManager, user.Role);
        Assert.Equal("Victoria Island", user.Branch);
        Assert.True(user.IsRelationshipManager);
    }

    [Fact]
    public void Login_is_recorded_with_time_and_ip()
    {
        var user = TestUsers.Customer();

        user.RecordLogin("10.0.0.1", TestUsers.Now);

        Assert.Equal(TestUsers.Now, user.LastLoginAt);
        Assert.Equal("10.0.0.1", user.LastLoginIp);
    }
}
