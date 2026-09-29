using RmConnect.Domain.Appointments;
using RmConnect.Domain.Common;
using RmConnect.Domain.Relationships;

namespace RmConnect.UnitTests.Domain;

public class AppointmentTests
{
    private static readonly DateTime Tomorrow = TestUsers.Now.AddDays(1);

    [Fact]
    public void Booking_with_the_active_manager_creates_a_booked_appointment()
    {
        var relationship = TestUsers.ActiveRelationship();

        var appointment = Appointment.Book(relationship, Tomorrow, " Loan options ", AppointmentChannel.Call, TestUsers.Now);

        Assert.Equal(AppointmentStatus.Booked, appointment.Status);
        Assert.Equal(relationship.ManagerId, appointment.ManagerId);
        Assert.Equal("Loan options", appointment.Reason);
    }

    [Fact]
    public void Cannot_book_while_the_request_is_still_pending()
    {
        var pending = Relationship.Request(TestUsers.Customer(), TestUsers.Manager(), TestUsers.Now);

        Assert.Throws<DomainException>(() => Appointment.Book(pending, Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now));
    }

    [Fact]
    public void Cannot_book_in_the_past()
    {
        var relationship = TestUsers.ActiveRelationship();

        Assert.Throws<DomainException>(() =>
            Appointment.Book(relationship, TestUsers.Now.AddHours(-1), "Loan", AppointmentChannel.Call, TestUsers.Now));
    }

    [Fact]
    public void A_booked_appointment_can_be_cancelled_once()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        appointment.Cancel();

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Throws<DomainException>(() => appointment.Cancel());
    }

    [Fact]
    public void Cannot_complete_before_it_starts()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        Assert.Throws<DomainException>(() => appointment.Complete(TestUsers.Now));
    }

    [Fact]
    public void Can_complete_once_it_has_started()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        appointment.Complete(Tomorrow.AddMinutes(30));

        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Reschedule_moves_a_booked_appointment()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        appointment.Reschedule(Tomorrow.AddHours(2), TestUsers.Now);

        Assert.Equal(Tomorrow.AddHours(2), appointment.StartsAt);
    }

    [Fact]
    public void Cannot_reschedule_to_the_same_time()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        Assert.Throws<DomainException>(() => appointment.Reschedule(Tomorrow, TestUsers.Now));
    }

    [Fact]
    public void Cannot_reschedule_after_it_has_started()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);

        Assert.Throws<DomainException>(() => appointment.Reschedule(Tomorrow.AddDays(1), Tomorrow.AddMinutes(5)));
    }

    [Fact]
    public void Cannot_reschedule_a_cancelled_appointment()
    {
        var appointment = Appointment.Book(TestUsers.ActiveRelationship(), Tomorrow, "Loan", AppointmentChannel.Call, TestUsers.Now);
        appointment.Cancel();

        Assert.Throws<DomainException>(() => appointment.Reschedule(Tomorrow.AddHours(2), TestUsers.Now));
    }
}
