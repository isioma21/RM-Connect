namespace RmConnect.Application.Sessions;

/// <summary>The "Auth" section of appsettings: how long a device stays signed in without activity.</summary>
public class SessionOptions
{
    public const string SectionName = "Auth";

    public int SessionMinutes { get; set; }
}
