using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public UserService(
        IUserRepository userRepo,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepo;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
            throw new UnauthorizedAccessException("Access denied");

        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null)
            throw new KeyNotFoundException("User not found");
        return ToProfileDto(user);
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateUserRequest request)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
            throw new UnauthorizedAccessException("Access denied");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                throw new ArgumentException("Email already in use");

            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            user.ProfilePictureUrl = request.ProfilePictureUrl;

        await _userRepository.UpdateAsync(user);
    }
    
    public UserProfileDto ToProfileDto(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}