using FluentValidation;
using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Validation.Validators.Match;

public class MatchQueryValidator : AbstractValidator<MatchQuery>
{
    public MatchQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100");

        RuleFor(x => x.Q)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Q));

        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To)
            .WithMessage("From date must be less than or equal to To date")
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}
