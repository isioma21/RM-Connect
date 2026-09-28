using RmConnect.Domain.Common;
using RmConnect.Domain.Relationships;
using RmConnect.Domain.Users;

namespace RmConnect.Domain.Appointments;

/// <summary>A call or branch visit between a customer and their relationship manager.</summary>
public class Appointment
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ManagerId { get; private set; }
    public DateTime StartsAt { get; private set; }
    public string Reason { get; private set; } = default!;
    public AppointmentChannel Channel { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User Customer { get; private set; } = default!;
    public User Manager { get; private set; } = default!;

    private Appointment() { }

    /// <summary>Books a meeting within an active relationship.</summary>
    public static Appointment Book(Relationship relationship, DateTime startsAt, string reason, AppointmentChannel channel, DateTime now)
    {
        if (relationship.Status != RelationshipStatus.Active)
            throw new DomainException("You can only book with your active relationship manager.");
        if (startsAt <= now) throw new DomainException("The appointment time must be in the future.");

        return new Appointment
        {
            Id = Guid.NewGuid(),
            CustomerId = relationship.CustomerId,
            ManagerId = relationship.ManagerId,
            StartsAt = startsAt,
            Reason = reason.Trim(),
            Channel = channel,
            Status = AppointmentStatus.Booked,
            CreatedAt = now
        };
    }

    public void Cancel()
    {
        if (Status != AppointmentStatus.Booked) throw new DomainException("Only booked appointments can be cancelled.");
        Status = AppointmentStatus.Cancelled;
    }

    public void Complete(DateTime now)
    {
        if (Status != AppointmentStatus.Booked) throw new DomainException("Only booked appointments can be completed.");
        if (StartsAt > now) throw new DomainException("An appointment can't be completed before it starts.");
        Status = AppointmentStatus.Completed;
    }
}
