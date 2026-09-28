using RmConnect.Domain.Users;

namespace RmConnect.Application.Auth;

public record UserResponse(Guid Id, string FirstName, string LastName, string Email, string? Phone, UserRole Role)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.FirstName, user.LastName, user.Email, user.Phone, user.Role);
}
