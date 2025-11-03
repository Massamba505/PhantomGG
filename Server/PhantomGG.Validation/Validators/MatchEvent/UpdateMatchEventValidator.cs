using FluentValidation;
using PhantomGG.Models.DTOs.MatchEvent;

namespace PhantomGG.Validation.Validators.MatchEvent;

public class UpdateMatchEventValidator : AbstractValidator<UpdateMatchEventDto>
{
    public UpdateMatchEventValidator()
    {
        RuleFor(x => x.EventType)
            .IsInEnum()
            .WithMessage("Invalid match event type specified");

        RuleFor(x => x.Minute)
            .InclusiveBetween(0, 120)
            .WithMessage("Minute must be between 0 and 120");

        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("Player ID cannot be empty");
    }
}