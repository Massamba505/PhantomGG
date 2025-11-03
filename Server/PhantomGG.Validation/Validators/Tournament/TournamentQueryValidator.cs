using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class TournamentQueryValidator : AbstractValidator<TournamentQuery>
{
    public TournamentQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid tournament status specified");
    }
}
