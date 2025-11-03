using FluentValidation;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Validation.Validators.Match;

public class MatchQueryValidator : AbstractValidator<MatchQuery>
{
    public MatchQueryValidator()
    {
        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To)
            .WithMessage("From date must be less than or equal to To date")
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}
