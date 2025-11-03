using FluentValidation;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Validation.Regex;

namespace PhantomGG.Validation.Validators.Team;

public class CreateTeamValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required")
            .Length(2, 200)
            .WithMessage("Team name must be between 2 and 200 characters")
            .Matches(RegexPatterns.AlphanumericPattern)
            .WithMessage("Team name contains invalid characters");

        RuleFor(x => x.ShortName)
            .NotEmpty()
            .WithMessage("Short team name is required")
            .Length(3, 10)
            .WithMessage("Team name must be between 3 and 10 characters");

        RuleFor(x => x.LogoUrl)
            .Must(BeValidImageFile)
            .WithMessage("File must be a valid image (jpg, jpeg, png)")
            .Must(BeValidFileSize)
            .WithMessage("File size cannot exceed 5MB");
    }

    private static bool BeValidImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
        return allowedTypes.Contains(file.ContentType?.ToLower());
    }

    private static bool BeValidFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        const long maxSizeInBytes = 5 * 1024 * 1024;
        return file.Length <= maxSizeInBytes;
    }
}
