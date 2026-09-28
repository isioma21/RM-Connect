namespace RmConnect.Domain.Users;

/// <summary>A customer or a relationship manager.</summary>
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Branch { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? LastLoginIp { get; private set; }

    private User() { }

    /// <summary>Customers sign up with their name, email and phone number.</summary>
    public static User RegisterCustomer(string firstName, string lastName, string email, string phone,
        string passwordHash, DateTime now)
    {
        var user = Create(firstName, lastName, email, passwordHash, UserRole.Customer, now);
        user.Phone = phone.Trim();
        return user;
    }

    /// <summary>Relationship managers sign up with their name, work email and the branch they work at.</summary>
    public static User RegisterManager(string firstName, string lastName, string workEmail, string branch,
        string passwordHash, DateTime now)
    {
        var user = Create(firstName, lastName, workEmail, passwordHash, UserRole.RelationshipManager, now);
        user.Branch = branch.Trim();
        return user;
    }

    public bool IsRelationshipManager => Role == UserRole.RelationshipManager;

    public void RecordLogin(string ipAddress, DateTime now)
    {
        LastLoginAt = now;
        LastLoginIp = ipAddress;
    }

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static User Create(string firstName, string lastName, string email, string passwordHash,
        UserRole role, DateTime now)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = now
        };
    }
}
