using FluentValidation;

namespace RmConnect.Application.Auth;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(r => r.CurrentPassword).NotEmpty();
        RuleFor(r => r.NewPassword).StrongPassword()
            .NotEqual(r => r.CurrentPassword).WithMessage("New password must be different from the current one.");
    }
}
