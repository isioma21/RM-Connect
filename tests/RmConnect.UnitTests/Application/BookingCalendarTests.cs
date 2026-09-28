using Microsoft.Extensions.Options;
using RmConnect.Application.Appointments;

namespace RmConnect.UnitTests.Application;

public class BookingCalendarTests
{
    // Lagos is UTC+1 all year, so 09:00 in Lagos is 08:00 UTC
    private readonly BookingCalendar _calendar = new(Options.Create(new AppointmentOptions
    {
        TimeZone = "Africa/Lagos",
        FirstSlotHour = 9,
        LastSlotHour = 16
    }));

    private static readonly DateOnly Wednesday = new(2026, 9, 30);
    private static readonly DateOnly Saturday = new(2026, 10, 3);

    [Fact]
    public void Weekday_has_hourly_slots_from_9_to_16_bank_time()
    {
        var slots = _calendar.SlotsOn(Wednesday);

        Assert.Equal(8, slots.Count);
        Assert.Equal(new DateTime(2026, 9, 30, 8, 0, 0, DateTimeKind.Utc), slots.First());
        Assert.Equal(new DateTime(2026, 9, 30, 15, 0, 0, DateTimeKind.Utc), slots.Last());
    }

    [Fact]
    public void Weekend_has_no_slots()
    {
        Assert.Empty(_calendar.SlotsOn(Saturday));
    }

    [Theory]
    [InlineData(8, 0, true)]    // 09:00 Lagos
    [InlineData(15, 0, true)]   // 16:00 Lagos, the last slot
    [InlineData(7, 0, false)]   // 08:00 Lagos, before opening
    [InlineData(16, 0, false)]  // 17:00 Lagos, after the last slot
    [InlineData(9, 30, false)]  // 10:30 Lagos, not on the hour
    public void Only_weekday_hours_on_the_hour_are_slots(int utcHour, int minute, bool expected)
    {
        var startsAt = new DateTime(2026, 9, 30, utcHour, minute, 0, DateTimeKind.Utc);

        Assert.Equal(expected, _calendar.IsSlot(startsAt));
    }

    [Fact]
    public void Saturday_morning_is_not_a_slot()
    {
        Assert.False(_calendar.IsSlot(new DateTime(2026, 10, 3, 8, 0, 0, DateTimeKind.Utc)));
    }
}
