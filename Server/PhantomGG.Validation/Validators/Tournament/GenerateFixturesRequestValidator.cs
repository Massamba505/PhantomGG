using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class GenerateFixturesRequestValidator : AbstractValidator<GenerateFixturesRequest>
{
    public GenerateFixturesRequestValidator()
    {
        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("Invalid tournament format specified");
    }
}