using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Image;

namespace PhantomGG.Service.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<string> UploadImageAsync(UploadImageRequest uploadImage);
}
