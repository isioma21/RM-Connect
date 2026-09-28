using System.Net;
using System.Net.Http.Json;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class AuthTests(ApiFactory api)
{
    [Fact]
    public async Task Customer_can_register()
    {
        var response = await api.CreateClient().PostAsJsonAsync("/api/auth/register/customer", new
        {
            firstName = "Emeka", lastName = "Obi", email = "emeka.obi@example.com", phone = "07012345678", password = ApiFactory.Password
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Registering_an_existing_email_returns_409()
    {
        var customer = await api.CreateCustomerAsync();

        var response = await api.CreateClient().PostAsJsonAsync("/api/auth/register/customer", new
        {
            firstName = "Copy", lastName = "Cat", email = customer.Email, phone = "07012345678", password = ApiFactory.Password
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Wrong_password_returns_401()
    {
        var customer = await api.CreateCustomerAsync();

        var response = await api.CreateClient().PostAsJsonAsync("/api/auth/login", new { email = customer.Email, password = "Wrong123!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logged_in_user_can_get_their_details()
    {
        var customer = await api.CreateCustomerAsync();

        var response = await customer.Client.GetAsync("/api/auth/current-user");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task After_logout_the_user_is_logged_out()
    {
        var customer = await api.CreateCustomerAsync();

        await customer.Client.PostAsync("/api/auth/logout", null);
        var response = await customer.Client.GetAsync("/api/auth/current-user");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Protected_endpoints_need_a_login()
    {
        var response = await api.CreateClient().GetAsync("/api/relationships/managers");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
