using FluentValidation;
using UserAccessManager.Core.DTOs.Request;

namespace UserAccessManager.Core.Validators;

public class CreateAccessRequestValidator : AbstractValidator<CreateAccessRequest>
{
    public CreateAccessRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(255).WithMessage("UserName must not exceed 255 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.AppRoleId)
            .GreaterThan(0).WithMessage("AppRoleId must be a positive integer.");
    }
}
