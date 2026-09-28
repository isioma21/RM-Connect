using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RmConnect.Application.Appointments;
using RmConnect.Application.Auth;
using RmConnect.Application.Relationships;

namespace RmConnect.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RegistrationOptions>(configuration.GetSection(RegistrationOptions.SectionName));
        services.Configure<AppointmentOptions>(configuration.GetSection(AppointmentOptions.SectionName));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddSingleton<BookingCalendar>();
        services.AddScoped<AuthService>();
        services.AddScoped<RelationshipService>();
        services.AddScoped<AppointmentService>();

        return services;
    }
}
