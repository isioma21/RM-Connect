using System.Net;
using System.Net.Http.Json;
using RmConnect.Application.Appointments;

namespace RmConnect.IntegrationTests;

[Collection("Api")]
public class RescheduleTests(ApiFactory api)
{
    // Wednesday 9 January 2030, Lagos time (UTC+1)
    private static readonly DateTimeOffset Slot = new(2030, 1, 9, 10, 0, 0, TimeSpan.FromHours(1));
    private static readonly DateTimeOffset NewSlot = new(2030, 1, 9, 11, 0, 0, TimeSpan.FromHours(1));

    [Fact]
    public async Task Rescheduling_moves_the_appointment_and_frees_the_old_slot()
    {
        var (customer, _, appointment) = await BookAsync();

        var response = await customer.Client.PostAsJsonAsync($"/api/appointments/{appointment.Id}/reschedule", new { startsAt = NewSlot });
        var moved = await response.ReadAsync<AppointmentResponse>();
        var freeSlots = await (await customer.Client.GetAsync("/api/appointments/slots?date=2030-01-09")).ReadAsync<List<DateTime>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(NewSlot.UtcDateTime, moved.StartsAt);
        Assert.Contains(Slot.UtcDateTime, freeSlots);
        Assert.DoesNotContain(NewSlot.UtcDateTime, freeSlots);
    }

    [Fact]
    public async Task Cannot_reschedule_to_a_taken_slot()
    {
        var (customer, manager, appointment) = await BookAsync();
        var otherCustomer = await api.CreateCustomerAsync();
        await api.ConnectAsync(otherCustomer, manager);
        await otherCustomer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = NewSlot, channel = "Call", reason = "Loan" });

        var response = await customer.Client.PostAsJsonAsync($"/api/appointments/{appointment.Id}/reschedule", new { startsAt = NewSlot });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Manager_cannot_reschedule()
    {
        var (_, manager, appointment) = await BookAsync();

        var response = await manager.Client.PostAsJsonAsync($"/api/appointments/{appointment.Id}/reschedule", new { startsAt = NewSlot });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<(TestUser Customer, TestUser Manager, AppointmentResponse Appointment)> BookAsync()
    {
        var customer = await api.CreateCustomerAsync();
        var manager = await api.CreateManagerAsync();
        await api.ConnectAsync(customer, manager);

        var booking = await customer.Client.PostAsJsonAsync("/api/appointments", new { startsAt = Slot, channel = "Call", reason = "Loan" });
        return (customer, manager, await booking.ReadAsync<AppointmentResponse>());
    }
}
