using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class CreateTournamentValidator : AbstractValidator<CreateTournamentDto>
{
    public CreateTournamentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tournament name is required")
            .Length(3, 200).WithMessage("Tournament name must be between 3 and 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_\.]+$").WithMessage("Tournament name contains invalid characters");

        RuleFor(x => x.Location)
            .Length(0, 200).WithMessage("Location must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.RegistrationStartDate)
            .NotEmpty().WithMessage("Registration start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Registration start date cannot be in the past");

        RuleFor(x => x.RegistrationDeadline)
            .NotEmpty().WithMessage("Registration deadline is required")
            .GreaterThan(x => x.RegistrationStartDate).WithMessage("Registration deadline must be after registration start date");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Tournament start date is required")
            .GreaterThan(x => x.RegistrationDeadline).WithMessage("Tournament start date must be after registration deadline");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Tournament end date is required")
            .GreaterThan(x => x.StartDate).WithMessage("Tournament end date must be after start date");

        RuleFor(x => x.MinTeams)
            .GreaterThanOrEqualTo(2).WithMessage("Minimum teams must be at least 2")
            .LessThanOrEqualTo(64).WithMessage("Minimum teams cannot exceed 64");

        RuleFor(x => x.MaxTeams)
            .GreaterThanOrEqualTo(4).WithMessage("Maximum teams must be at least 4")
            .LessThanOrEqualTo(128).WithMessage("Maximum teams cannot exceed 128")
            .GreaterThan(x => x.MinTeams).WithMessage("Maximum teams must be greater than minimum teams");

        RuleFor(x => x.BannerUrl)
            .Must(BeValidImageFile).WithMessage("Banner must be a valid image file (jpg, jpeg, png, gif)")
            .When(x => x.BannerUrl != null);

        RuleFor(x => x.LogoUrl)
            .Must(BeValidImageFile).WithMessage("Logo must be a valid image file (jpg, jpeg, png, gif)")
            .When(x => x.LogoUrl != null);
    }

    private bool BeValidImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(extension) && file.Length > 0 && file.Length <= 5 * 1024 * 1024;
    }
}
