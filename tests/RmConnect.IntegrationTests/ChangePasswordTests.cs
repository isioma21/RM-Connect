using System.Net;
using System.Net.Http.Json;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class ChangePasswordTests(ApiFactory api)
{
    private const string NewPassword = "NewPassword456!";

    [Fact]
    public async Task Changing_password_signs_out_every_device_and_needs_the_new_password()
    {
        var laptop = await api.CreateCustomerAsync();
        var phone = await api.LoginAsync(laptop.Email);

        var response = await laptop.Client.PostAsJsonAsync("/api/auth/change-password",
            new { currentPassword = ApiFactory.Password, newPassword = NewPassword });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await laptop.Client.GetAsync("/api/auth/current-user")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await phone.Client.GetAsync("/api/auth/current-user")).StatusCode);

        var oldLogin = await api.CreateClient().PostAsJsonAsync("/api/auth/login", new { email = laptop.Email, password = ApiFactory.Password });
        var newLogin = await api.CreateClient().PostAsJsonAsync("/api/auth/login", new { email = laptop.Email, password = NewPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLogin.StatusCode);
        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
    }

    [Fact]
    public async Task Wrong_current_password_returns_400()
    {
        var user = await api.CreateCustomerAsync();

        var response = await user.Client.PostAsJsonAsync("/api/auth/change-password",
            new { currentPassword = "WrongPassword1", newPassword = NewPassword });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Current password is incorrect.", await response.Content.ReadAsStringAsync());
    }
}
