using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class TeamManagementRequestValidator : AbstractValidator<TeamManagementRequest>
{
    public TeamManagementRequestValidator()
    {
        RuleFor(x => x.Action)
            .IsInEnum()
            .WithMessage("Invalid team action specified");
    }
}