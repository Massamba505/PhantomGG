using PhantomGG.API.Common;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class LocalFileImageService(
    IWebHostEnvironment webHostEnvironment,
    IHttpContextAccessor httpContextAccessor) : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly string[] _supportedTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024;

    public async Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null)
    {
        ValidateFile(file);

        var folderName = GetFolderName(imageType);
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = GenerateFileName(file.FileName, entityId);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return GenerateImageUrl(folderName, fileName);
    }

    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return Task.FromResult(false);

        try
        {
            var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
            var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : imageUrl;

            relativePath = relativePath.TrimStart('/');
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ValidationException("No file provided");
        }

        if (!_supportedTypes.Contains(file.ContentType))
        {
            throw new ValidationException("Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed");
        }

        if (file.Length > _maxFileSize)
        {
            throw new ValidationException("File size cannot exceed 5MB");
        }
    }

    private string GetFolderName(ImageType imageType)
    {
        return imageType switch
        {
            ImageType.profilePicture => "profilepictures",
            ImageType.TournamentBanner => "banners",
            ImageType.Logo => "logos",
            _ => "misc"
        };
    }

    private string GenerateFileName(string originalFileName, Guid? entityId)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var uniqueId = entityId?.ToString("N") ?? Guid.NewGuid().ToString("N");
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return $"{uniqueId}_{timestamp}{extension}";
    }

    private string GenerateImageUrl(string folderName, string fileName)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/images/{folderName}/{fileName}";
        }

        return $"/images/{folderName}/{fileName}";
    }
}
