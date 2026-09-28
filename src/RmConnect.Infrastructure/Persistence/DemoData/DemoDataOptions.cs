namespace RmConnect.Infrastructure.Persistence.DemoData;

/// <summary>The "DemoData" section of appsettings.</summary>
public class DemoDataOptions
{
    public const string SectionName = "DemoData";

    public bool Enabled { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
