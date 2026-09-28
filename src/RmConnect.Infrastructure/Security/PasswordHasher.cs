using Microsoft.AspNetCore.Identity;
using RmConnect.Application.Common.Interfaces;

namespace RmConnect.Infrastructure.Security;

/// <summary>Uses ASP.NET Core Identity's hasher so we don't write our own crypto.</summary>
public class PasswordHasher : IPasswordHasher
{
    // Identity's hasher asks for a user object but never uses it, so we pass null
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string hash, string password) =>
        _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
