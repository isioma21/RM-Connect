using System.Net;
using System.Net.Http.Json;
using RmConnect.Application.Relationships;
using RmConnect.Domain.Relationships;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class RelationshipTests(ApiFactory api)
{
    [Fact]
    public async Task Relationship_is_active_after_the_manager_accepts()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();

        await api.ConnectAsync(customer, manager);
        var response = await customer.Client.GetAsync("/api/relationships/current");
        var relationship = await response.ReadAsync<RelationshipResponse>();

        Assert.Equal(RelationshipStatus.Active, relationship.Status);
    }

    [Fact]
    public async Task Customer_cannot_request_a_second_manager()
    {
        var customer = await api.CreateCustomerAsync();
        var firstManager = await api.CreateManagerAsync();
        var secondManager = await api.CreateManagerAsync();

        await customer.Client.PostAsJsonAsync("/api/relationships", new { managerId = firstManager.Id });
        var response = await customer.Client.PostAsJsonAsync("/api/relationships", new { managerId = secondManager.Id });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Another_manager_cannot_accept_the_request()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();
        var otherManager = await api.CreateManagerAsync();

        var request = await customer.Client.PostAsJsonAsync("/api/relationships", new { managerId = manager.Id });
        var relationship = await request.ReadAsync<RelationshipResponse>();
        var response = await otherManager.Client.PostAsync($"/api/relationships/{relationship.Id}/accept", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Customer_cannot_accept_a_request()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();

        var request = await customer.Client.PostAsJsonAsync("/api/relationships", new { managerId = manager.Id });
        var relationship = await request.ReadAsync<RelationshipResponse>();
        var response = await customer.Client.PostAsync($"/api/relationships/{relationship.Id}/accept", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
