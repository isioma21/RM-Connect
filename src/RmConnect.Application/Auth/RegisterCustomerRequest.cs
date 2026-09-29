using FluentValidation;

namespace RmConnect.Application.Auth;

public record RegisterCustomerRequest(string FirstName, string LastName, string Email, string Phone, string Password);

public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.LastName).NotEmpty().MaximumLength(50);
        RuleFor(r => r.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(r => r.Phone).NotEmpty()
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number must be 10 to 15 digits.");
        RuleFor(r => r.Password).StrongPassword();
    }
}
