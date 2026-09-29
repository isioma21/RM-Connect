using RmConnect.Application.Auth;

namespace RmConnect.UnitTests.Application;

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void Valid_change_passes()
    {
        Assert.True(_validator.Validate(new ChangePasswordRequest("Password1", "NewPassword2")).IsValid);
    }

    [Theory]
    [InlineData("short1A")]
    [InlineData("nouppercase1")]
    [InlineData("NoNumberHere")]
    public void New_password_must_be_strong(string newPassword)
    {
        Assert.False(_validator.Validate(new ChangePasswordRequest("Password1", newPassword)).IsValid);
    }

    [Fact]
    public void New_password_must_be_different()
    {
        var result = _validator.Validate(new ChangePasswordRequest("Password1", "Password1"));

        Assert.Contains(result.Errors, e => e.ErrorMessage == "New password must be different from the current one.");
    }
}
