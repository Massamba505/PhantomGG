using Microsoft.AspNetCore.Http;
using PhantomGG.Service.Exceptions;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Common.Enums;
using PhantomGG.Service.Mappings;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Repository.Entities;
using PhantomGG.Service.Infrastructure.Security.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Domain.Users.Interfaces;

namespace PhantomGG.Service.Domain.Users.Implementations;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IImageService imageService,
    HybridCache cache) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly HybridCache _cache = cache;

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var cacheKey = $"user_{id}";
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10)
        };

        return await _cache.GetOrCreateAsync(cacheKey, async _ =>
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return user.ToDto();
        }, options);
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        await ValidateEmailUniquenessForUserAsync(request.Email, userId);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user);
        return user.ToDto();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        await ValidateCurrentPasswordAsync(user, request.CurrentPassword);

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

    private async Task ValidateEmailUniquenessForUserAsync(string email, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user!.Email != email)
        {
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new ConflictException("Email address is already in use");
            }
        }
    }

    private Task ValidateCurrentPasswordAsync(User user, string currentPassword)
    {
        var validPassword = _passwordHasher.VerifyPassword(currentPassword, user.PasswordHash);
        if (!validPassword)
        {
            throw new ValidationException("Current password is incorrect");
        }
        return Task.CompletedTask;
    }
}