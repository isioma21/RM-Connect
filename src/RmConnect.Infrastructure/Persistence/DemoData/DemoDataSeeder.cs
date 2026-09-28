using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Users;

namespace RmConnect.Infrastructure.Persistence.DemoData;

/// <summary>When DemoData:Enabled is true, registers the users in the demo JSON file (skipping emails already taken).</summary>
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

        var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, options.FilePath));
        var file = JsonSerializer.Deserialize<DemoUsersFile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        var passwordHash = hasher.Hash(options.Password);
        var now = DateTime.UtcNow;
        var added = 0;

        foreach (var manager in file.Managers)
        {
            if (await IsRegistered(db, manager.Email)) continue;
            db.Users.Add(User.RegisterManager(manager.FirstName, manager.LastName, manager.Email, passwordHash, now));
            added++;
        }

        foreach (var customer in file.Customers)
        {
            if (await IsRegistered(db, customer.Email)) continue;
            db.Users.Add(User.RegisterCustomer(customer.FirstName, customer.LastName, customer.Email, customer.Phone, passwordHash, now));
            added++;
        }

        await db.SaveChangesAsync();
        logger.LogInformation("Demo data: {AddedCount} users added", added);
    }

    private static Task<bool> IsRegistered(AppDbContext db, string email) =>
        db.Users.AnyAsync(u => u.Email == User.NormalizeEmail(email));
}
