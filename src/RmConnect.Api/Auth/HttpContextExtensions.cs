namespace RmConnect.Api.Auth;

public static class HttpContextExtensions
{
    public static string GetClientIp(this HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    /// <summary>The browser or app the request came from (User-Agent header).</summary>
    public static string GetDevice(this HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        return string.IsNullOrWhiteSpace(userAgent) ? "Unknown device" : userAgent;
    }
}
