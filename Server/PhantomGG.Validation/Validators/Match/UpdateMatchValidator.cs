using FluentValidation;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Validation.Validators.Match;

public class UpdateMatchValidator : AbstractValidator<UpdateMatchDto>
{
    public UpdateMatchValidator()
    {
        RuleFor(x => x.MatchDate)
            .NotEmpty()
            .WithMessage("Match date is required")
            .GreaterThan(DateTime.UtcNow.AddHours(-2))
            .WithMessage("Match date cannot be too far in the past");

        RuleFor(x => x.Venue)
            .MaximumLength(200)
            .WithMessage("Venue cannot exceed 200 characters");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required");
    }
}
