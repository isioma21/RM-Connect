using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RmConnect.Application.Common.Exceptions;
using RmConnect.Application.Common.Interfaces;
using RmConnect.Domain.Appointments;
using RmConnect.Domain.Relationships;

namespace RmConnect.Application.Appointments;

public class AppointmentService(
    IAppDbContext db,
    BookingCalendar calendar,
    IValidator<BookAppointmentRequest> validator,
    ILogger<AppointmentService> logger)
{
    public async Task<List<DateTime>> GetFreeSlotsAsync(Guid customerId, DateOnly date, CancellationToken ct)
    {
        var relationship = await GetActiveRelationshipAsync(customerId, ct);
        var slots = calendar.SlotsOn(date);

        var bookedSlots = await db.Appointments
            .Where(a => a.ManagerId == relationship.ManagerId && a.Status == AppointmentStatus.Booked && slots.Contains(a.StartsAt))
            .Select(a => a.StartsAt)
            .ToListAsync(ct);

        return slots.Where(slot => slot > DateTime.UtcNow && !bookedSlots.Contains(slot)).ToList();
    }

    public async Task<AppointmentResponse> BookAsync(Guid customerId, BookAppointmentRequest request, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var relationship = await GetActiveRelationshipAsync(customerId, ct);
        var startsAt = request.StartsAt.UtcDateTime;

        var slotTaken = await db.Appointments.AnyAsync(a => a.ManagerId == relationship.ManagerId &&
            a.StartsAt == startsAt && a.Status == AppointmentStatus.Booked, ct);
        if (slotTaken)
            throw new ConflictException("This time slot is no longer available.");

        var appointment = Appointment.Book(relationship, startsAt, request.Reason, request.Channel, DateTime.UtcNow);
        db.Appointments.Add(appointment);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Another booking for the same slot was saved first; the unique index rejected this one.
            throw new ConflictException("This time slot is no longer available.");
        }

        logger.LogInformation("Customer {CustomerId} booked a {Channel} for {StartsAt} (appointment {AppointmentId})",
            customerId, request.Channel, startsAt, appointment.Id);
        return AppointmentResponse.From(appointment);
    }

    public async Task<List<AppointmentResponse>> GetMineAsync(Guid userId, CancellationToken ct)
    {
        var appointments = await db.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Manager)
            .Where(a => a.CustomerId == userId || a.ManagerId == userId)
            .OrderBy(a => a.StartsAt)
            .ToListAsync(ct);

        return appointments.Select(AppointmentResponse.From).ToList();
    }

    public async Task<AppointmentResponse> CancelAsync(Guid userId, Guid appointmentId, CancellationToken ct)
    {
        var appointment = await FindAsync(appointmentId, userId, ct);
        appointment.Cancel();
        await db.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} cancelled appointment {AppointmentId}", userId, appointmentId);
        return AppointmentResponse.From(appointment);
    }

    public async Task<AppointmentResponse> CompleteAsync(Guid managerId, Guid appointmentId, CancellationToken ct)
    {
        var appointment = await FindAsync(appointmentId, managerId, ct);
        appointment.Complete(DateTime.UtcNow);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Manager {ManagerId} completed appointment {AppointmentId}", managerId, appointmentId);
        return AppointmentResponse.From(appointment);
    }

    private async Task<Relationship> GetActiveRelationshipAsync(Guid customerId, CancellationToken ct)
    {
        var relationship = await db.Relationships
            .Include(r => r.Customer)
            .Include(r => r.Manager)
            .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.Status == RelationshipStatus.Active, ct);

        return relationship ?? throw new NotFoundException("You need an active relationship manager to book an appointment.");
    }

    private async Task<Appointment> FindAsync(Guid appointmentId, Guid userId, CancellationToken ct)
    {
        var appointment = await db.Appointments
            .Include(a => a.Customer)
            .Include(a => a.Manager)
            .FirstOrDefaultAsync(a => a.Id == appointmentId && (a.CustomerId == userId || a.ManagerId == userId), ct);

        return appointment ?? throw new NotFoundException("Appointment not found.");
    }
}
