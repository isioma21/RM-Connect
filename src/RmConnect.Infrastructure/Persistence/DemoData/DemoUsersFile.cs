namespace RmConnect.Infrastructure.Persistence.DemoData;

/// <summary>Shape of the demo users JSON file.</summary>
public class DemoUsersFile
{
    public List<DemoManager> Managers { get; set; } = [];
    public List<DemoCustomer> Customers { get; set; } = [];
}

public record DemoManager(string FirstName, string LastName, string Email);

public record DemoCustomer(string FirstName, string LastName, string Email, string Phone);
