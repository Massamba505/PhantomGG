using FluentValidation;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Validation.Validators.Team;

public class TeamQueryValidator : AbstractValidator<TeamQuery>
{
    public TeamQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid role specified");
    }
}
