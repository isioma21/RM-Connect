using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RmConnect.Application.Common.Exceptions;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Users;

namespace RmConnect.Application.Auth;

public class AuthService(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    IValidator<RegisterCustomerRequest> customerValidator,
    IValidator<RegisterManagerRequest> managerValidator,
    ILogger<AuthService> logger)
{
    public async Task<UserResponse> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken ct)
    {
        await customerValidator.ValidateAndThrowAsync(request, ct);

        var user = User.RegisterCustomer(request.FirstName, request.LastName, request.Email, request.Phone,
            passwordHasher.Hash(request.Password), DateTime.UtcNow);

        return await SaveNewUserAsync(user, ct);
    }

    public async Task<UserResponse> RegisterManagerAsync(RegisterManagerRequest request, CancellationToken ct)
    {
        await managerValidator.ValidateAndThrowAsync(request, ct);

        var user = User.RegisterManager(request.FirstName, request.LastName, request.WorkEmail, request.Branch,
            passwordHasher.Hash(request.Password), DateTime.UtcNow);

        return await SaveNewUserAsync(user, ct);
    }

    public async Task<UserResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = User.NormalizeEmail(request.Email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            logger.LogWarning("Login failed for {Email}", email);
            return null;
        }

        logger.LogInformation("User {UserId} logged in as {Role}", user.Id, user.Role);
        return UserResponse.From(user);
    }

    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user is null ? null : UserResponse.From(user);
    }

    private async Task<UserResponse> SaveNewUserAsync(User user, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Email == user.Email, ct))
        {
            logger.LogWarning("Registration rejected: {Email} is already registered", user.Email);
            throw new ConflictException("An account with this email already exists.");
        }

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("{Role} {UserId} registered with {Email}", user.Role, user.Id, user.Email);
        return UserResponse.From(user);
    }
}
