using System.Net;
using RmConnect.Application.Sessions;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class SessionTests(ApiFactory api)
{
    [Fact]
    public async Task Signing_out_other_devices_keeps_this_one_logged_in()
    {
        var laptop = await api.CreateCustomerAsync();
        var phone = await api.LoginAsync(laptop.Email);

        await laptop.Client.PostAsync("/api/sessions/revoke-others", null);
        var phoneResponse = await phone.Client.GetAsync("/api/auth/current-user");
        var laptopResponse = await laptop.Client.GetAsync("/api/auth/current-user");

        Assert.Equal(HttpStatusCode.Unauthorized, phoneResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, laptopResponse.StatusCode);
    }

    [Fact]
    public async Task User_cannot_sign_out_another_users_device()
    {
        var me = await api.CreateCustomerAsync();
        var someoneElse = await api.CreateCustomerAsync();

        var theirResponse = await someoneElse.Client.GetAsync("/api/sessions");
        var theirSessions = await theirResponse.ReadAsync<List<SessionResponse>>();
        var response = await me.Client.DeleteAsync($"/api/sessions/{theirSessions[0].Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
