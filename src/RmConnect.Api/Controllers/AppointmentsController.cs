using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RmConnect.Api.Auth;
using RmConnect.Application.Appointments;
using RmConnect.Domain.Users;

namespace RmConnect.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/appointments")]
public class AppointmentsController(AppointmentService appointmentService) : ControllerBase
{
    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpGet("slots")]
    public async Task<ActionResult<List<DateTime>>> GetFreeSlots([BindRequired] DateOnly date, CancellationToken ct)
    {
        return await appointmentService.GetFreeSlotsAsync(User.GetUserId(), date, ct);
    }

    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Book(BookAppointmentRequest request, CancellationToken ct)
    {
        var appointment = await appointmentService.BookAsync(User.GetUserId(), request, ct);
        return CreatedAtAction(nameof(GetMine), appointment);
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentResponse>>> GetMine(CancellationToken ct)
    {
        return await appointmentService.GetMineAsync(User.GetUserId(), ct);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<AppointmentResponse>> Cancel(Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] CancelAppointmentRequest? request, CancellationToken ct)
    {
        return await appointmentService.CancelAsync(User.GetUserId(), id, request ?? new CancelAppointmentRequest(null), ct);
    }

    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpPost("{id:guid}/reschedule")]
    public async Task<ActionResult<AppointmentResponse>> Reschedule(Guid id, RescheduleAppointmentRequest request, CancellationToken ct)
    {
        return await appointmentService.RescheduleAsync(User.GetUserId(), id, request, ct);
    }

    [Authorize(Roles = nameof(UserRole.RelationshipManager))]
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<AppointmentResponse>> Complete(Guid id, CancellationToken ct)
    {
        return await appointmentService.CompleteAsync(User.GetUserId(), id, ct);
    }
}
