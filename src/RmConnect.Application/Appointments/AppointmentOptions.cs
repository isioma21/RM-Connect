namespace RmConnect.Application.Appointments;

/// <summary>The "Appointments" section of appsettings: the bank's time zone and opening hours for bookings.</summary>
public class AppointmentOptions
{
    public const string SectionName = "Appointments";

    public string TimeZone { get; set; } = string.Empty;
    public int FirstSlotHour { get; set; }
    public int LastSlotHour { get; set; }
}
