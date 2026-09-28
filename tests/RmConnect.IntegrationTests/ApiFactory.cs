using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RmConnect.Application.Auth;
using RmConnect.Application.Relationships;
using RmConnect.Infrastructure.Persistence;

namespace RmConnect.IntegrationTests;

/// <summary>A logged-in user: their own HttpClient (it keeps the login cookie), id and email.</summary>
public record TestUser(HttpClient Client, Guid Id, string Email);

/// <summary>Starts the API in memory against a fresh test database.</summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string Password = "Password123!";

    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=RmConnect_Tests;Trusted_Connection=True;TrustServerCertificate=True";

    public ApiFactory()
    {
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).Options);
        db.Database.EnsureDeleted();
        db.Database.Migrate();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", ConnectionString);
    }

    public async Task<TestUser> CreateCustomerAsync()
    {
        var email = $"customer-{Guid.NewGuid():N}@example.com";
        await CreateClient().PostAsJsonAsync("/api/auth/register/customer",
            new { firstName = "Test", lastName = "Customer", email, phone = "08031234567", password = Password });

        return await LoginAsync(email);
    }

    public async Task<TestUser> CreateManagerAsync()
    {
        var email = $"manager-{Guid.NewGuid():N}@rmconnect.bank";
        await CreateClient().PostAsJsonAsync("/api/auth/register/manager",
            new { firstName = "Test", lastName = "Manager", workEmail = email, branch = "Victoria Island", password = Password });

        return await LoginAsync(email);
    }

    /// <summary>Logs in from a new client, like a separate browser or device.</summary>
    public async Task<TestUser> LoginAsync(string email)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = Password });
        var user = await response.ReadAsync<UserResponse>();

        return new TestUser(client, user.Id, user.Email);
    }

    /// <summary>The customer requests the manager and the manager accepts.</summary>
    public async Task ConnectAsync(TestUser customer, TestUser manager)
    {
        var request = await customer.Client.PostAsJsonAsync("/api/relationships", new { managerId = manager.Id });
        var relationship = await request.ReadAsync<RelationshipResponse>();

        await manager.Client.PostAsync($"/api/relationships/{relationship.Id}/accept", null);
    }
}

[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<ApiFactory>;
