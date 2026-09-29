using RmConnect.Domain.Common;
using RmConnect.Domain.Users;

namespace RmConnect.Domain.Relationships;

/// <summary>A customer's link to a relationship manager: requested, then accepted or declined.</summary>
public class Relationship
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ManagerId { get; private set; }
    public RelationshipStatus Status { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public string? EndReason { get; private set; }

    public User Customer { get; private set; } = default!;
    public User Manager { get; private set; } = default!;

    private Relationship() { }

    public static Relationship Request(User customer, User manager, DateTime now)
    {
        if (customer.IsRelationshipManager) throw new DomainException("Only customers can request a relationship manager.");
        if (!manager.IsRelationshipManager) throw new DomainException("The selected user is not a relationship manager.");

        return new Relationship
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            ManagerId = manager.Id,
            Status = RelationshipStatus.Pending,
            RequestedAt = now
        };
    }

    public void Accept(DateTime now)
    {
        EnsurePending("Only pending requests can be accepted.");
        Status = RelationshipStatus.Active;
        RespondedAt = now;
    }

    public void Decline(DateTime now)
    {
        EnsurePending("Only pending requests can be declined.");
        Status = RelationshipStatus.Declined;
        RespondedAt = now;
    }

    /// <summary>Cancels a pending request or ends an active relationship.</summary>
    public void End(DateTime now, string? reason = null)
    {
        if (!IsOpen) throw new DomainException("This relationship has already been closed.");
        Status = RelationshipStatus.Ended;
        EndedAt = now;
        EndReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    /// <summary>Pending or active: a customer can have only one open relationship at a time.</summary>
    public bool IsOpen => Status is RelationshipStatus.Pending or RelationshipStatus.Active;

    private void EnsurePending(string message)
    {
        if (Status != RelationshipStatus.Pending) throw new DomainException(message);
    }
}
