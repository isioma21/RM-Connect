using FluentValidation;

namespace RmConnect.Application.Auth;

public static class PasswordRules
{
    /// <summary>8 to 72 characters with an uppercase letter, a lowercase letter and a number.</summary>
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().MinimumLength(8).MaximumLength(72)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a number.");
}
