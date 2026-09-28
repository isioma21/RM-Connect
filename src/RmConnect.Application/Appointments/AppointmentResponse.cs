using RmConnect.Application.Auth;
using RmConnect.Domain.Appointments;

namespace RmConnect.Application.Appointments;

public record AppointmentResponse(
    Guid Id,
    DateTime StartsAt,
    AppointmentChannel Channel,
    string Reason,
    AppointmentStatus Status,
    UserResponse Customer,
    UserResponse Manager)
{
    public static AppointmentResponse From(Appointment appointment) =>
        new(appointment.Id, appointment.StartsAt, appointment.Channel, appointment.Reason, appointment.Status,
            UserResponse.From(appointment.Customer), UserResponse.From(appointment.Manager));
}
