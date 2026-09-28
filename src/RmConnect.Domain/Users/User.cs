using RmConnect.Domain.Common;

namespace RmConnect.Domain.Users;

/// <summary>A customer or a relationship manager.</summary>
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User() { } 

    /// <summary>Customers sign up with their name, email and phone number.</summary>
    public static User RegisterCustomer(string firstName, string lastName, string email, string phone,
        string passwordHash, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(phone)) throw new DomainException("Phone number is required.");

        var user = Create(firstName, lastName, email, passwordHash, UserRole.Customer, now);
        user.Phone = phone.Trim();
        return user;
    }

    /// <summary>Relationship managers sign up with their name and work email.</summary>
    public static User RegisterManager(string firstName, string lastName, string workEmail,
        string passwordHash, DateTime now)
    {
        return Create(firstName, lastName, workEmail, passwordHash, UserRole.RelationshipManager, now);
    }

    public bool IsRelationshipManager => Role == UserRole.RelationshipManager;

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static User Create(string firstName, string lastName, string email, string passwordHash,
        UserRole role, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("Last name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Password is required.");

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
