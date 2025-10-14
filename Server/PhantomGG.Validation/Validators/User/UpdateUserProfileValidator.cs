using FluentValidation;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Validation.Regex;

namespace PhantomGG.Validation.Validators.User;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileValidator()
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
    }
}
