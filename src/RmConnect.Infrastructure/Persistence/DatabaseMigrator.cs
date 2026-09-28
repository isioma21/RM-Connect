using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RmConnect.Infrastructure.Persistence;

/// <summary>When Database:MigrateOnStartup is true (Docker), applies pending migrations before the API starts.</summary>
public static class DatabaseMigrator
{
    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        if (!services.GetRequiredService<IConfiguration>().GetValue<bool>("Database:MigrateOnStartup")) return;

        using var scope = services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }
}
