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
            .GreaterThan(DateTime.UtcNow.AddHours(-1))
            .WithMessage("Match date cannot be in the past");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .IsInEnum()
            .WithMessage("Invalid role specified");
    }
}
