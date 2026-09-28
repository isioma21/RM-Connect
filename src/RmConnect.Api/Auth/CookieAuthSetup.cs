using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RmConnect.Application.Sessions;

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

                options.Events.OnValidatePrincipal = async context =>
                {
                    var sessionId = context.Principal?.FindFirstValue(ClaimsPrincipalExtensions.SessionIdClaim);
                    var sessions = context.HttpContext.RequestServices.GetRequiredService<SessionService>();

                    if (sessionId is null || !await sessions.IsActiveAsync(Guid.Parse(sessionId), context.HttpContext.RequestAborted))
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                };
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
