using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PhantomGG.Common.Config;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;

namespace PhantomGG.Service.Infrastructure.Storage.Implementations;

public class AzureBlobImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string? _publicBlobBaseUrl;

    public AzureBlobImageService(IOptions<StorageSettings> storageSettings)
    {
        var settings = storageSettings.Value;

        if (string.IsNullOrEmpty(settings.AzureStorageConnectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not configured.");
        }

        _blobServiceClient = new BlobServiceClient(settings.AzureStorageConnectionString);
        _containerName = settings.BlobContainerName;
        _publicBlobBaseUrl = string.IsNullOrWhiteSpace(settings.PublicBlobUrl)
            ? null
            : settings.PublicBlobUrl.TrimEnd('/');
    }

    public async Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null)
    {
        var folderName = GetFolderName(imageType);
        var fileName = GenerateFileName(file.FileName, entityId);
        var blobName = $"{folderName}/{fileName}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(blobName);

        var contentType = GetContentType(file.FileName);
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeaders
        });

        if (!string.IsNullOrEmpty(_publicBlobBaseUrl))
        {
            return $"{_publicBlobBaseUrl}/{_containerName}/{blobName}";
        }

        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath.TrimStart('/');

            string blobName;
            var containerSegment = $"{_containerName}/";
            var idx = path.IndexOf(containerSegment, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                blobName = path.Substring(idx + containerSegment.Length);
            }
            else if (path.StartsWith(_containerName + "/", StringComparison.OrdinalIgnoreCase))
            {
                blobName = path.Substring(_containerName.Length + 1);
            }
            else
            {
                blobName = path;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
        catch
        {
            return false;
        }
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

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
