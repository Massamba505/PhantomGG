using PhantomGG.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Service.Exceptions;

namespace PhantomGG.Service.Implementations;

public class LocalFileImageService(
    IWebHostEnvironment webHostEnvironment,
    IHttpContextAccessor httpContextAccessor) : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
    private readonly string[] _supportedTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };

    public async Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null)
    {
        ValidateFile(file);

        var folderName = GetFolderName(imageType);
        var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var fileName = GenerateFileName(file.FileName, entityId);
        var filePath = Path.Combine(uploadsPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return GenerateImageUrl(folderName, fileName);
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
        var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : imageUrl;

        if (relativePath.StartsWith("/images/"))
        {
            var localPath = relativePath.Replace("/images/", "").Replace("/", "\\");
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", localPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return await Task.Run(() =>
                {
                    return true;
                });
            }
        }

        return await Task.Run(() =>
        {
            return false;
        });
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
            ImageType.ProfilePicture => "profilepictures",
            ImageType.TournamentBanner => "banners",
            ImageType.TournamentLogo => "logos",
            ImageType.TeamLogo => "teamlogos",
            ImageType.TeamPhoto => "teamphotos",
            ImageType.PlayerPhoto => "playerphotos",
            _ => "uploads"
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
