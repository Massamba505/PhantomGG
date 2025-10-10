using FluentValidation;
using PhantomGG.Models.DTOs.Team;

namespace PhantomGG.Validation.Validators.Team;

public class CreateTeamValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .Length(2, 50).WithMessage("Team name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_\.]+$").WithMessage("Team name contains invalid characters");

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
