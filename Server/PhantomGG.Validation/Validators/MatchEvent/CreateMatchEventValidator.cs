using FluentValidation;
using PhantomGG.Models.DTOs.MatchEvent;

namespace PhantomGG.Validation.Validators.MatchEvent;

public class CreateMatchEventValidator : AbstractValidator<CreateMatchEventDto>
{
    public CreateMatchEventValidator()
    {
        RuleFor(x => x.MatchId)
            .NotEmpty()
            .WithMessage("Match ID is required");

        RuleFor(x => x.EventType)
            .IsInEnum()
            .WithMessage("Invalid match event type specified");

        RuleFor(x => x.Minute)
            .InclusiveBetween(0, 120)
            .WithMessage("Minute must be between 0 and 120");

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required");

        RuleFor(x => x.PlayerId)
            .NotEmpty()
            .WithMessage("Player ID is required");
    }
}