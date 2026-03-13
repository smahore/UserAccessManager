using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.Validators;

namespace UserAccessManager.API.Tests.Validators;

public class CreateAccessRequestValidatorTests
{
    private readonly CreateAccessRequestValidator _validator = new();

    [Fact]
    public async Task WhenAllFieldsAreValidThenPasses()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "user1@test.com", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task WhenUserNameIsEmptyThenFails()
    {
        var request = new CreateAccessRequest { UserName = "", Email = "user1@test.com", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "UserName is required.");
    }

    [Fact]
    public async Task WhenUserNameExceedsMaxLengthThenFails()
    {
        var request = new CreateAccessRequest { UserName = new string('a', 256), Email = "user1@test.com", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("255"));
    }

    [Fact]
    public async Task WhenEmailIsEmptyThenFails()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Email is required.");
    }

    [Fact]
    public async Task WhenEmailIsInvalidThenFails()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "not-valid", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("valid email"));
    }

    [Fact]
    public async Task WhenEmailExceedsMaxLengthThenFails()
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = new string('a', 250) + "@t.com", AppRoleId = 3 };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("255"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task WhenAppRoleIdIsNotPositiveThenFails(int appRoleId)
    {
        var request = new CreateAccessRequest { UserName = "user1", Email = "user1@test.com", AppRoleId = appRoleId };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "AppRoleId must be a positive integer.");
    }
}
