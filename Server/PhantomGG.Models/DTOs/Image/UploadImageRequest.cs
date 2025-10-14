using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;

namespace PhantomGG.Models.DTOs.Image;

public class UploadImageRequest
{
    public string? OldFileUrl { get; set; }
    public required IFormFile File { get; set; }
    public Guid Id { get; set; }
    public ImageType ImageType { get; set; }
}