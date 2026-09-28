using RmConnect.Domain.Relationships;
using RmConnect.Domain.Users;

namespace RmConnect.UnitTests;

public static class TestUsers
{
    public static readonly DateTime Now = new(2026, 9, 28, 10, 0, 0, DateTimeKind.Utc);

    public static User Customer() =>
        User.RegisterCustomer("Chidi", "Nwosu", "chidi@example.com", "08031234567", "hash", Now);

    public static User Manager() =>
        User.RegisterManager("Adaeze", "Okafor", "adaeze.okafor@rmconnect.bank", "Victoria Island", "hash", Now);

    public static Relationship ActiveRelationship()
    {
        var relationship = Relationship.Request(Customer(), Manager(), Now);
        relationship.Accept(Now);
        return relationship;
    }
}
