using RmConnect.Domain.Common;
using RmConnect.Domain.Relationships;

namespace RmConnect.UnitTests.Domain;

public class RelationshipTests
{
    [Fact]
    public void New_request_is_pending()
    {
        var relationship = Relationship.Request(TestUsers.Customer(), TestUsers.Manager(), TestUsers.Now);

        Assert.Equal(RelationshipStatus.Pending, relationship.Status);
        Assert.Equal(TestUsers.Now, relationship.RequestedAt);
    }

    [Fact]
    public void Customer_cannot_request_another_customer()
    {
        Assert.Throws<DomainException>(() => Relationship.Request(TestUsers.Customer(), TestUsers.Customer(), TestUsers.Now));
    }

    [Fact]
    public void Manager_cannot_request_a_manager()
    {
        Assert.Throws<DomainException>(() => Relationship.Request(TestUsers.Manager(), TestUsers.Manager(), TestUsers.Now));
    }

    [Fact]
    public void Accepting_a_pending_request_makes_it_active()
    {
        var relationship = Relationship.Request(TestUsers.Customer(), TestUsers.Manager(), TestUsers.Now);

        relationship.Accept(TestUsers.Now);

        Assert.Equal(RelationshipStatus.Active, relationship.Status);
        Assert.Equal(TestUsers.Now, relationship.RespondedAt);
    }

    [Fact]
    public void Declining_a_pending_request_makes_it_declined()
    {
        var relationship = Relationship.Request(TestUsers.Customer(), TestUsers.Manager(), TestUsers.Now);

        relationship.Decline(TestUsers.Now);

        Assert.Equal(RelationshipStatus.Declined, relationship.Status);
    }

    [Fact]
    public void Only_pending_requests_can_be_accepted_or_declined()
    {
        var relationship = TestUsers.ActiveRelationship();

        Assert.Throws<DomainException>(() => relationship.Accept(TestUsers.Now));
        Assert.Throws<DomainException>(() => relationship.Decline(TestUsers.Now));
    }

    [Fact]
    public void Ending_an_active_relationship_closes_it()
    {
        var relationship = TestUsers.ActiveRelationship();

        relationship.End(TestUsers.Now);

        Assert.Equal(RelationshipStatus.Ended, relationship.Status);
        Assert.False(relationship.IsOpen);
    }

    [Fact]
    public void A_closed_relationship_cannot_be_ended_again()
    {
        var relationship = TestUsers.ActiveRelationship();
        relationship.End(TestUsers.Now);

        Assert.Throws<DomainException>(() => relationship.End(TestUsers.Now));
    }
}
