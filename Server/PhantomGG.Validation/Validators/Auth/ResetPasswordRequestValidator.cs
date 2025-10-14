using FluentValidation;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Validation.Regex;

namespace PhantomGG.Validation.Validators.Auth;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required")
            .MaximumLength(255)
            .WithMessage("Invalid Token");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters")
            .Matches(RegexPatterns.PasswordPattern)
            .WithMessage("Password must be at least 8 characters and include upper/lowercase, digit, and symbol");
    }
}