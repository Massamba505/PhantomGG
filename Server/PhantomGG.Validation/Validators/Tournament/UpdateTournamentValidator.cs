using FluentValidation;
using PhantomGG.Models.DTOs.Tournament;

namespace PhantomGG.Validation.Validators.Tournament;

public class UpdateTournamentValidator : AbstractValidator<UpdateTournamentDto>
{
    public UpdateTournamentValidator()
    {
        RuleFor(x => x.Name)
            .Length(3, 200).WithMessage("Tournament name must be between 3 and 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_\.]+$").WithMessage("Tournament name contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .Length(10, 2000).WithMessage("Tournament description must be between 10 and 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Location)
            .Length(0, 200).WithMessage("Location must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.RegistrationDeadline)
            .GreaterThan(DateTime.UtcNow).WithMessage("Registration deadline must be in the future")
            .When(x => x.RegistrationDeadline.HasValue);

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Tournament start date must be in the future")
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x.MaxTeams)
            .GreaterThanOrEqualTo(4).WithMessage("Maximum teams must be at least 4")
            .LessThanOrEqualTo(128).WithMessage("Maximum teams cannot exceed 128")
            .When(x => x.MaxTeams.HasValue);

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Contact email must be a valid email address")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));

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
