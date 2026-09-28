using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Infrastructure.Persistence;
using RmConnect.Infrastructure.Persistence.DemoData;
using RmConnect.Infrastructure.Security;

namespace RmConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.Configure<DemoDataOptions>(configuration.GetSection(DemoDataOptions.SectionName));

        return services;
    }
}
