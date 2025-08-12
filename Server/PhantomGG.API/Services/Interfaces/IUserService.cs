using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.Services.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task UpdateUserProfileAsync(Guid userId, UpdateUserRequest request);
}
