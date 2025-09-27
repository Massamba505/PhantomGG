using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;

namespace PhantomGG.Service.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null);
    Task<bool> DeleteImageAsync(string imageUrl);
}
