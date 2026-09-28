using Microsoft.AspNetCore.Authentication.Cookies;

namespace RmConnect.Api.Auth;

public static class CookieAuthSetup
{
    public static IServiceCollection AddCookieAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var sessionLength = TimeSpan.FromMinutes(configuration.GetValue<int>("Auth:SessionMinutes"));

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = configuration["Auth:CookieName"];
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = sessionLength;
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();
        return services;
    }
}
