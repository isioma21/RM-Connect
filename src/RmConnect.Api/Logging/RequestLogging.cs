using System.Security.Claims;
using RmConnect.Api.Auth;
using Serilog;

namespace RmConnect.Api.Logging;

public static class RequestLogging
{
    /// <summary>One log line per request: method, path, status, time taken, who made it and from which IP.</summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0} ms (user {UserId}, IP {ClientIp})";

            options.EnrichDiagnosticContext = (diagnostics, context) =>
            {
                diagnostics.Set("UserId", context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous");
                diagnostics.Set("ClientIp", context.GetClientIp());
            };
        });
    }
}
