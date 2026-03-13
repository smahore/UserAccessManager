using FluentValidation;
using UserAccessManager.Core.DTOs.Request;

namespace UserAccessManager.Core.Validators;

public class UpdateStatusRequestValidator : AbstractValidator<UpdateStatusRequest>
{
    private static readonly string[] ValidStatuses = ["Approved", "Rejected", "Cancelled", "Fulfilled"];

    public UpdateStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.");
    }
}
