using FluentValidation;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Validation.Validators.Team;

public class TeamQueryValidator : AbstractValidator<TeamQuery>
{
    public TeamQueryValidator()
    {
        RuleFor(x => x.Q)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters");

        RuleFor(x => x.Status)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}
