using FluentValidation;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Validation.Regex;

namespace PhantomGG.Validation.Validators.Player;

public class UpdatePlayerValidator : AbstractValidator<UpdatePlayerDto>
{
    public UpdatePlayerValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters")
            .Matches(RegexPatterns.FullNamePattern)
            .WithMessage("FirstName can only contain letters, spaces, hyphens, or apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .Length(1, 50)
            .WithMessage("Last name must be between 1 and 50 characters")
            .Matches(RegexPatterns.FullNamePattern)
            .WithMessage("LastName can only contain letters, spaces, hyphens, or apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Position)
            .IsInEnum()
            .WithMessage("Invalid player position specified");

        RuleFor(x => x.PhotoUrl)
            .NotNull()
            .WithMessage("File is required")
            .Must(BeValidProfileImageFile)
            .WithMessage("Photo must be a valid image file (jpg, jpeg, png, gif)")
            .Must(BeValidProfileImageFileSize)
            .WithMessage("File size cannot exceed 5MB");
    }

    private static bool BeValidProfileImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        return allowedTypes.Contains(file.ContentType?.ToLower());
    }

    private static bool BeValidProfileImageFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return false;

        const long maxSizeInBytes = 5 * 1024 * 1024;
        return file.Length <= maxSizeInBytes;
    }
}