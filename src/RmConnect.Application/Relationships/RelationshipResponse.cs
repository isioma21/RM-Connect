using RmConnect.Application.Auth;
using RmConnect.Domain.Relationships;

namespace RmConnect.Application.Relationships;

public record RelationshipResponse(
    Guid Id,
    RelationshipStatus Status,
    DateTime RequestedAt,
    DateTime? RespondedAt,
    DateTime? EndedAt,
    string? EndReason,
    UserResponse Customer,
    UserResponse Manager)
{
    public static RelationshipResponse From(Relationship relationship) =>
        new(relationship.Id, relationship.Status, relationship.RequestedAt, relationship.RespondedAt, relationship.EndedAt, relationship.EndReason,
            UserResponse.From(relationship.Customer), UserResponse.From(relationship.Manager));
}
