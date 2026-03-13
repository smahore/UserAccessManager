using FluentValidation;
using UserAccessManager.Core.DTOs.Request;

namespace UserAccessManager.Core.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(150).WithMessage("UserName must not exceed 150 characters.");

        RuleFor(x => x.FullName)
            .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.")
            .When(x => x.FullName != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(300).WithMessage("Email must not exceed 300 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required.")
            .MaximumLength(20).WithMessage("Source must not exceed 20 characters.");
    }
}
