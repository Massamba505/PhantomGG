using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class TournamentQueryValidator : AbstractValidator<TournamentQuery>
{
    public TournamentQueryValidator()
    {
        RuleFor(x => x.Q)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters");

        RuleFor(x => x.Status)
            .NotEmpty();

        RuleFor(x => x.Location)
            .MaximumLength(100)
            .WithMessage("Location cannot exceed 100 characters");

        RuleFor(x => x.StartFrom)
            .LessThanOrEqualTo(x => x.StartTo)
            .When(x => x.StartFrom.HasValue && x.StartTo.HasValue)
            .WithMessage("Start date must be before or equal to end date");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        var validStatuses = new[] { "upcoming", "ongoing", "completed" };
        return validStatuses.Contains(status.ToLower());
    }
}
