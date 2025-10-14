using FluentValidation;
using PhantomGG.Models.DTOs.Auth;
using PhantomGG.Validation.Regex;

namespace PhantomGG.Validation.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters")
            .Matches(RegexPatterns.FullNamePattern)
            .WithMessage("FirstName can only contain letters, spaces, hyphens, or apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .Length(1, 50)
            .WithMessage("Last name must be between 1 and 50 characters")
            .Matches(RegexPatterns.FullNamePattern)
            .WithMessage("LastName can only contain letters, spaces, hyphens, or apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MaximumLength(255)
            .WithMessage("Password cannot exceed 255 characters")
            .Matches(RegexPatterns.PasswordPattern)
            .WithMessage("Password must be at least 8 characters and include upper/lowercase, digit, and symbol");

        RuleFor(x => x.ProfilePictureUrl)
            .MaximumLength(255)
            .WithMessage("Profile picture URL cannot exceed 255 characters");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid role specified");
    }
}