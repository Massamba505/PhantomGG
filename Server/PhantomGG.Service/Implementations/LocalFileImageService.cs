using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class LocalFileImageService(
    IWebHostEnvironment webHostEnvironment,
    IHttpContextAccessor httpContextAccessor) : IImageService
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null)
    {
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

        var imageUrl = GenerateImageUrl(folderName, fileName);

        return imageUrl;
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

    public async Task<string> UploadImageAsync(UploadImageRequest uploadImage)
    {
        if (!string.IsNullOrEmpty(uploadImage.OldFileUrl))
        {
            await DeleteImageAsync(uploadImage.OldFileUrl);
        }

        var imageUrl = await SaveImageAsync(uploadImage.File, uploadImage.ImageType, uploadImage.Id);

        return imageUrl;
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
