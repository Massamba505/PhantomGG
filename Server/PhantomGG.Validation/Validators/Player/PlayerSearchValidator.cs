using FluentValidation;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Validation.Validators.Player;

public class PlayerSearchValidator : AbstractValidator<PlayerSearchDto>
{
    public PlayerSearchValidator()
    {
        RuleFor(x => x.Position)
            .IsInEnum()
            .WithMessage("Invalid player position specified");
    }
}