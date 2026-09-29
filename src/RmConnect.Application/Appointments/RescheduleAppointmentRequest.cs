using FluentValidation;

namespace RmConnect.Application.Appointments;

public record RescheduleAppointmentRequest(DateTimeOffset StartsAt);

public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
{
    public RescheduleAppointmentRequestValidator(BookingCalendar calendar)
    {
        RuleFor(r => r.StartsAt)
            .Must(startsAt => calendar.IsSlot(startsAt.UtcDateTime))
            .WithMessage($"Pick a time slot: {calendar.OpeningHours}.");
    }
}
