using FluentValidation;

namespace RmConnect.Application.Appointments;

/// <summary>Managers must give a reason so the customer knows why; customers may leave it empty.</summary>
public record CancelAppointmentRequest(string? Reason);

public class CancelAppointmentRequestValidator : AbstractValidator<CancelAppointmentRequest>
{
    public CancelAppointmentRequestValidator()
    {
        RuleFor(r => r.Reason).MaximumLength(200);
    }
}
