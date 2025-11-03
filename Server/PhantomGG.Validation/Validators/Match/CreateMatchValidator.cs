using FluentValidation;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Validation.Validators.Match;

public class CreateMatchValidator : AbstractValidator<CreateMatchDto>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.TournamentId)
            .NotEmpty()
            .WithMessage("Tournament ID is required");

        RuleFor(x => x.HomeTeamId)
            .NotEmpty()
            .WithMessage("Home team ID is required");

        RuleFor(x => x.AwayTeamId)
            .NotEmpty()
            .WithMessage("Away team ID is required")
            .NotEqual(x => x.HomeTeamId)
            .WithMessage("Home team and away team cannot be the same");

        RuleFor(x => x.MatchDate)
            .NotEmpty()
            .WithMessage("Match date is required")
            .GreaterThan(DateTime.UtcNow.AddHours(-1))
            .WithMessage("Match date cannot be in the past");

        RuleFor(x => x.Venue)
            .MaximumLength(200)
            .WithMessage("Venue cannot exceed 200 characters");
    }
}
