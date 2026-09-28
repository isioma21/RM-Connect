using System.Net;
using System.Net.Http.Json;
using RmConnect.Application.Appointments;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class AppointmentTests(ApiFactory api)
{
    // Tuesday 8 January 2030, 10:00 Lagos time (UTC+1)
    private static readonly DateTimeOffset Slot = new(2030, 1, 8, 10, 0, 0, TimeSpan.FromHours(1));

    [Fact]
    public async Task Booked_slot_is_no_longer_free()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();
        await api.ConnectAsync(customer, manager);

        var booking = await customer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });
        var slots = await customer.Client.GetAsync("/api/appointments/slots?date=2030-01-08");
        var freeSlots = await slots.ReadAsync<List<DateTime>>();

        Assert.Equal(HttpStatusCode.Created, booking.StatusCode);
        Assert.DoesNotContain(Slot.UtcDateTime, freeSlots);
    }

    [Fact]
    public async Task Same_slot_cannot_be_booked_twice()
    {
        var manager = await api.CreateManagerAsync();
        var firstCustomer = await api.CreateCustomerAsync();
        var secondCustomer = await api.CreateCustomerAsync();
        await api.ConnectAsync(firstCustomer, manager);
        await api.ConnectAsync(secondCustomer, manager);

        await firstCustomer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });
        var response = await secondCustomer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Customer_without_a_manager_cannot_book()
    {
        var customer = await api.CreateCustomerAsync();

        var response = await customer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Another_customer_cannot_cancel_the_appointment()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();
        var stranger = await api.CreateCustomerAsync();
        await api.ConnectAsync(customer, manager);

        var booking = await customer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });
        var appointment = await booking.ReadAsync<AppointmentResponse>();
        var response = await stranger.Client.PostAsync($"/api/appointments/{appointment.Id}/cancel", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
