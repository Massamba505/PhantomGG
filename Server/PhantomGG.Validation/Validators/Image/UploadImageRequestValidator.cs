using FluentValidation;
using PhantomGG.Models.DTOs.Image;

namespace PhantomGG.Validation.Validators.Image;

public class UploadImageRequestValidator : AbstractValidator<UploadImageRequest>
{
    public UploadImageRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required")
            .Must(BeValidImageFile)
            .WithMessage("File must be a valid image (jpg, jpeg, png)")
            .Must(BeValidFileSize)
            .WithMessage("File size cannot exceed 5MB");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID is required");

        RuleFor(x => x.ImageType)
            .IsInEnum()
            .WithMessage("Invalid image type specified");

        RuleFor(x => x.OldFileUrl)
            .Must(BeValidUrl)
            .WithMessage("Old file URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.OldFileUrl));

        RuleFor(x => x.ImageType)
            .IsInEnum()
            .WithMessage("Invalid image type specified");
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

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}