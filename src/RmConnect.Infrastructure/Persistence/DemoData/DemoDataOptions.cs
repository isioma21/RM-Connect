namespace RmConnect.Infrastructure.Persistence.DemoData;

/// <summary>The "DemoData" section: the flag and password from appsettings, the users from DemoData/demo-users.json.</summary>
public class DemoDataOptions
{
    public const string SectionName = "DemoData";

    public bool Enabled { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<DemoManager> Managers { get; set; } = [];
    public List<DemoCustomer> Customers { get; set; } = [];
}

public record DemoManager(string FirstName, string LastName, string Email);

public record DemoCustomer(string FirstName, string LastName, string Email, string Phone);
