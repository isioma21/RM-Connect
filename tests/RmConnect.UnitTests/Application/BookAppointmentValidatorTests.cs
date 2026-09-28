using Microsoft.Extensions.Options;
using RmConnect.Application.Appointments;
using RmConnect.Domain.Appointments;

namespace RmConnect.UnitTests.Application;

public class BookAppointmentValidatorTests
{
    private readonly BookAppointmentRequestValidator _validator = new(new BookingCalendar(Options.Create(new AppointmentOptions
    {
        TimeZone = "Africa/Lagos",
        FirstSlotHour = 9,
        LastSlotHour = 16
    })));

    // Wednesday 30 Sep 2026, 10:00 Lagos time
    private static readonly DateTimeOffset WednesdayTen = new(2026, 9, 30, 10, 0, 0, TimeSpan.FromHours(1));

    [Fact]
    public void Valid_booking_passes()
    {
        var request = new BookAppointmentRequest(WednesdayTen, AppointmentChannel.Call, "Loan options");

        Assert.True(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Same_time_sent_in_utc_is_also_valid()
    {
        var request = new BookAppointmentRequest(WednesdayTen.ToUniversalTime(), AppointmentChannel.Call, "Loan options");

        Assert.True(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Weekend_booking_is_rejected()
    {
        var saturday = new DateTimeOffset(2026, 10, 3, 10, 0, 0, TimeSpan.FromHours(1));

        Assert.False(_validator.Validate(new BookAppointmentRequest(saturday, AppointmentChannel.Call, "Loan")).IsValid);
    }

    [Fact]
    public void Reason_is_required()
    {
        Assert.False(_validator.Validate(new BookAppointmentRequest(WednesdayTen, AppointmentChannel.Call, "")).IsValid);
    }
}
