using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Users;

namespace RmConnect.Infrastructure.Persistence.DemoData;

/// <summary>When DemoData:Enabled is true, registers the demo users whose email isn't taken yet.</summary>
public static class DemoDataSeeder
{
    public static async Task SeedDemoDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DemoDataOptions>>().Value;
        if (!options.Enabled) return;

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DemoData");

        var passwordHash = hasher.Hash(options.Password);
        var now = DateTime.UtcNow;

        var demoUsers = options.Managers.Select(m => User.RegisterManager(m.FirstName, m.LastName, m.Email, passwordHash, now))
            .Concat(options.Customers.Select(c => User.RegisterCustomer(c.FirstName, c.LastName, c.Email, c.Phone, passwordHash, now)))
            .ToList();

        var demoEmails = demoUsers.Select(u => u.Email).ToList();
        var registeredEmails = await db.Users.Where(u => demoEmails.Contains(u.Email)).Select(u => u.Email).ToListAsync();

        var newUsers = demoUsers.Where(u => !registeredEmails.Contains(u.Email)).ToList();
        db.Users.AddRange(newUsers);
        await db.SaveChangesAsync();

        logger.LogInformation("Demo data: {AddedCount} users added", newUsers.Count);
    }
}
