using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<UserStatsDto> GetUserStatsAsync(Guid userId);
    Task<UserActivityDto> GetUserActivityAsync(Guid userId, int page, int limit);
    Task<ProfilePictureUploadDto> UploadProfilePictureAsync(Guid userId, IFormFile file);
}
