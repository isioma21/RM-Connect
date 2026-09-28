using FluentValidation;
using RmConnect.Domain.Appointments;

namespace RmConnect.Application.Appointments;

public record BookAppointmentRequest(DateTimeOffset StartsAt, AppointmentChannel Channel, string Reason);

public class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest>
{
    public BookAppointmentRequestValidator(BookingCalendar calendar)
    {
        RuleFor(r => r.StartsAt)
            .Must(startsAt => calendar.IsSlot(startsAt.UtcDateTime))
            .WithMessage($"Pick a time slot: {calendar.OpeningHours}.");
        RuleFor(r => r.Channel).IsInEnum();
        RuleFor(r => r.Reason).NotEmpty().MaximumLength(200);
    }
}
