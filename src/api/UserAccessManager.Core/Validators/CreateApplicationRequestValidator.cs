using FluentValidation;
using UserAccessManager.Core.DTOs.Request;

namespace UserAccessManager.Core.Validators;

public class CreateApplicationRequestValidator : AbstractValidator<CreateApplicationRequest>
{
    public CreateApplicationRequestValidator()
    {
        RuleFor(x => x.AppName)
            .NotEmpty().WithMessage("AppName is required.")
            .MaximumLength(100).WithMessage("AppName must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters.")
            .When(x => x.Description != null);
    }
}
