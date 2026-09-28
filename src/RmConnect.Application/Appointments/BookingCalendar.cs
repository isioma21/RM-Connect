using Microsoft.Extensions.Options;

namespace RmConnect.Application.Appointments;

/// <summary>Hourly slots on weekdays, in the bank's time zone. Times in and out are UTC.</summary>
public class BookingCalendar(IOptions<AppointmentOptions> options)
{
    private readonly AppointmentOptions _options = options.Value;
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZone);

    public List<DateTime> SlotsOn(DateOnly date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return [];

        var slots = new List<DateTime>();
        for (var hour = _options.FirstSlotHour; hour <= _options.LastSlotHour; hour++)
        {
            var localTime = date.ToDateTime(new TimeOnly(hour, 0));
            slots.Add(TimeZoneInfo.ConvertTimeToUtc(localTime, _timeZone));
        }

        return slots;
    }

    public bool IsSlot(DateTime startsAtUtc)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(startsAtUtc, _timeZone);
        return SlotsOn(DateOnly.FromDateTime(localTime)).Contains(startsAtUtc);
    }

    public string OpeningHours => $"weekdays {_options.FirstSlotHour:00}:00 to {_options.LastSlotHour:00}:00, on the hour";
}
