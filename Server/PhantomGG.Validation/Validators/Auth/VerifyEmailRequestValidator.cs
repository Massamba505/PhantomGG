using FluentValidation;
using PhantomGG.Models.DTOs.Auth;

namespace PhantomGG.Validation.Validators.Auth;

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required")
            .MaximumLength(255)
            .WithMessage("Invalid Token");
    }
}