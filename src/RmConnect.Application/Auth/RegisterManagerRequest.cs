using FluentValidation;
using Microsoft.Extensions.Options;

namespace RmConnect.Application.Auth;

public record RegisterManagerRequest(string FirstName, string LastName, string WorkEmail, string Password);

public class RegisterManagerRequestValidator : AbstractValidator<RegisterManagerRequest>
{
    public RegisterManagerRequestValidator(IOptions<RegistrationOptions> options)
    {
        var staffDomain = "@" + options.Value.StaffEmailDomain;

        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.LastName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.WorkEmail).NotEmpty().EmailAddress().MaximumLength(256)
            .Must(email => email.Trim().EndsWith(staffDomain, StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Use your work email ({staffDomain}).");
        RuleFor(r => r.Password).NotEmpty().MinimumLength(8).MaximumLength(72)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a number.");
    }
}
