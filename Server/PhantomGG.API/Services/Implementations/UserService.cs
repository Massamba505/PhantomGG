using PhantomGG.API.Common;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Mappings;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Security.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IImageService imageService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user.ToUserDto();
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (user.Email != request.Email)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new ConflictException("Email address is already in use");
            }
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user);
        return user.ToUserDto();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var validePassword = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash);
        if (!validePassword)
        {
            throw new ValidationException("Current password is incorrect");
        }

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task<ProfilePictureUploadDto> UploadProfilePictureAsync(Guid userId, IFormFile file)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            await _imageService.DeleteImageAsync(user.ProfilePictureUrl);
        }

        var imageUrl = await _imageService.SaveImageAsync(file, ImageType.ProfilePicture, userId);

        user.ProfilePictureUrl = imageUrl;
        await _userRepository.UpdateAsync(user);

        return new ProfilePictureUploadDto
        {
            ProfilePictureUrl = imageUrl
        };
    }
}