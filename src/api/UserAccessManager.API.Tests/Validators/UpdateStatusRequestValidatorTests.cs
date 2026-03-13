using UserAccessManager.Core.DTOs.Request;
using UserAccessManager.Core.Validators;

namespace UserAccessManager.API.Tests.Validators;

public class UpdateStatusRequestValidatorTests
{
    private readonly UpdateStatusRequestValidator _validator = new();

    [Theory]
    [InlineData("Approved")]
    [InlineData("Rejected")]
    [InlineData("Cancelled")]
    [InlineData("Fulfilled")]
    public async Task WhenStatusIsValidThenPasses(string status)
    {
        var request = new UpdateStatusRequest { Status = status };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task WhenStatusIsEmptyThenFails()
    {
        var request = new UpdateStatusRequest { Status = "" };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Status is required.");
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Unknown")]
    [InlineData("approved")]
    public async Task WhenStatusIsNotAllowedThenFails(string status)
    {
        var request = new UpdateStatusRequest { Status = status };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Status must be one of"));
    }
}
