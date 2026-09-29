using FluentValidation;

namespace RmConnect.Application.Relationships;

/// <summary>Optional feedback on why the relationship ended.</summary>
public record EndRelationshipRequest(string? Reason);

public class EndRelationshipRequestValidator : AbstractValidator<EndRelationshipRequest>
{
    public EndRelationshipRequestValidator()
    {
        RuleFor(r => r.Reason).MaximumLength(200);
    }
}
