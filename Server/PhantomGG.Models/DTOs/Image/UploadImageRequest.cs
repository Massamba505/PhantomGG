using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PhantomGG.Models.DTOs.Image;

public class UploadImageRequest
{
    public string? OldFileUrl { get; set; }

    [Required]
    public required IFormFile File { get; set; }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public ImageType ImageType { get; set; }
}
