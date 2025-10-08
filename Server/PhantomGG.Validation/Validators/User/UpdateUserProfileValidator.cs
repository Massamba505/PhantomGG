using FluentValidation;
using PhantomGG.Models.DTOs.User;

namespace PhantomGG.Validation.Validators.User;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .Length(1, 50)
            .WithMessage("First name must be between 1 and 50 characters")
            .Matches("^[a-zA-Z\\s'-]+$")
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .Length(1, 50)
            .WithMessage("Last name must be between 1 and 50 characters")
            .Matches("^[a-zA-Z\\s'-]+$")
            .WithMessage("Last name contains invalid characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .Length(1, 100)
            .WithMessage("Email must be between 1 and 100 characters");
    }
}
