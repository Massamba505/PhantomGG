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
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Domain.Users.Implementations;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IImageService imageService,
    IUserValidationService userValidationService,
    HybridCache cache) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserValidationService _userValidationService = userValidationService;
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
            var user = await _userValidationService.ValidateUserExistsAsync(id);
            return user.ToDto();
        }, options);
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userValidationService.ValidateUserIsActiveAsync(userId);

        var newFirstName = request.FirstName?.Trim();
        var newLastName = request.LastName?.Trim();
        var newEmail = request.Email?.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(newEmail) &&
            !string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _userRepository.EmailExistsAsync(newEmail);
            if (emailExists)
                throw new ConflictException("Email address is already in use");

            user.Email = newEmail;
        }

        if (!string.IsNullOrWhiteSpace(newFirstName) &&
            !string.Equals(user.FirstName, newFirstName, StringComparison.Ordinal))
        {
            user.FirstName = newFirstName;
        }

        if (!string.IsNullOrWhiteSpace(newLastName) &&
            !string.Equals(user.LastName, newLastName, StringComparison.Ordinal))
        {
            user.LastName = newLastName;
        }

        await _userRepository.UpdateAsync(user);

        await _cache.RemoveAsync($"user_{userId}");

        return user.ToDto();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userValidationService.ValidateUserIsActiveAsync(userId);
        await ValidateCurrentPasswordAsync(user, request.CurrentPassword);

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
        await _cache.RemoveAsync($"user_{userId}");
    }

    public async Task<ProfilePictureUploadDto> UploadProfilePictureAsync(Guid userId, IFormFile file)
    {
        var user = await _userValidationService.ValidateUserIsActiveAsync(userId);

        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            await _imageService.DeleteImageAsync(user.ProfilePictureUrl);
        }

        var imageUrl = await _imageService.SaveImageAsync(file, ImageType.ProfilePicture, userId);

        user.ProfilePictureUrl = imageUrl;
        await _userRepository.UpdateAsync(user);

        await _cache.RemoveAsync($"user_{userId}");

        return new ProfilePictureUploadDto
        {
            ProfilePictureUrl = imageUrl
        };
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