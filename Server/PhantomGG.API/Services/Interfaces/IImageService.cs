using PhantomGG.API.Common;

namespace PhantomGG.API.Services.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file, ImageType imageType, Guid? entityId = null);
    Task<bool> DeleteImageAsync(string imageUrl);
}
