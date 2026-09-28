using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using RmConnect.Api.Auth;
using RmConnect.Api.Errors;
using RmConnect.Api.Logging;
using RmConnect.Application;
using RmConnect.Infrastructure;
using RmConnect.Infrastructure.Persistence;
using RmConnect.Infrastructure.Persistence.DemoData;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("DemoData/demo-users.json", optional: true);

// Console + Seq, configured in appsettings.json ("Serilog" section)
builder.Host.UseSerilog((context, logger) => logger.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCookieAuth(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = InvalidRequestResponse.Create);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Behind nginx (Docker): take the client IP and scheme from X-Forwarded-* headers
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();
});

var app = builder.Build();

await app.Services.MigrateDatabaseAsync();
await app.Services.SeedDemoDataAsync();

if (app.Configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
    app.UseForwardedHeaders();

app.UseRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Lets the integration tests start the API in memory (WebApplicationFactory<Program>)
public partial class Program;
