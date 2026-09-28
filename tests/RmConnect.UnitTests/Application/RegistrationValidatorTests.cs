using Microsoft.Extensions.Options;
using RmConnect.Application.Auth;

namespace RmConnect.UnitTests.Application;

public class RegistrationValidatorTests
{
    private readonly RegisterCustomerRequestValidator _customerValidator = new();

    private readonly RegisterManagerRequestValidator _managerValidator =
        new(Options.Create(new RegistrationOptions { StaffEmailDomain = "rmconnect.bank" }));

    private static RegisterCustomerRequest ValidCustomer() =>
        new("Chidi", "Nwosu", "chidi@example.com", "08031234567", "Password1");

    private static RegisterManagerRequest ValidManager() =>
        new("Adaeze", "Okafor", "adaeze.okafor@rmconnect.bank", "Victoria Island", "Password1");

    [Fact]
    public void Valid_customer_passes()
    {
        Assert.True(_customerValidator.Validate(ValidCustomer()).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0803-123-4567")]
    [InlineData("12345")]
    public void Customer_phone_must_be_10_to_15_digits(string phone)
    {
        Assert.False(_customerValidator.Validate(ValidCustomer() with { Phone = phone }).IsValid);
    }

    [Theory]
    [InlineData("short1A")]      // under 8 characters
    [InlineData("password1")]    // no uppercase
    [InlineData("PASSWORD1")]    // no lowercase
    [InlineData("Passwordxx")]   // no number
    public void Password_must_be_strong(string password)
    {
        Assert.False(_customerValidator.Validate(ValidCustomer() with { Password = password }).IsValid);
    }

    [Fact]
    public void Password_longer_than_72_characters_is_rejected()
    {
        var password = "Aa1" + new string('x', 70);

        Assert.False(_customerValidator.Validate(ValidCustomer() with { Password = password }).IsValid);
    }

    [Fact]
    public void Valid_manager_passes()
    {
        Assert.True(_managerValidator.Validate(ValidManager()).IsValid);
    }

    [Theory]
    [InlineData("adaeze@gmail.com")]
    [InlineData("adaeze@fakermconnect.bank")]
    [InlineData("adaeze@gmail.com@rmconnect.bank")]
    public void Manager_must_use_the_staff_email_domain(string email)
    {
        Assert.False(_managerValidator.Validate(ValidManager() with { WorkEmail = email }).IsValid);
    }

    [Fact]
    public void Manager_branch_is_required()
    {
        Assert.False(_managerValidator.Validate(ValidManager() with { Branch = "" }).IsValid);
    }
}
