using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.User;

namespace PhantomGG.Service.Domain.Users.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ProfilePictureUploadDto> UploadProfilePictureAsync(Guid userId, IFormFile file);
}
