namespace RmConnect.Application.Auth;

/// <summary>The "Registration" section of appsettings.</summary>
public class RegistrationOptions
{
    public const string SectionName = "Registration";

    /// <summary>Relationship managers must register with an email on this domain, e.g. "rmconnect.bank".</summary>
    public string StaffEmailDomain { get; set; } = string.Empty;
}
