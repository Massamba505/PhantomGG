using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRefreshTokenService _refreshTokenService;

    public UserService(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        // Only admins should access all users
        if (_currentUserService.Role != "Admin")
            throw new UnauthorizedAccessException("Insufficient permissions");

        return await _userRepository.GetAllAsync();
    }

    public async Task UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        if (_currentUserService.UserId != userId && _currentUserService.Role != "Admin")
            throw new UnauthorizedAccessException("Cannot update other users");

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new ArgumentException("Email already in use");

            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            user.ProfilePictureUrl = request.ProfilePictureUrl;

        if (!string.IsNullOrEmpty(request.Role) && _currentUserService.Role == "Admin")
            user.Role = request.Role;

        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        if (_currentUserService.UserId != userId && _currentUserService.Role != "Admin")
            throw new UnauthorizedAccessException("Cannot delete other users");

        await _userRepository.DeleteAsync(userId);

        await _refreshTokenService.RevokeAllTokensForUserAsync(userId);
    }

    public async Task UpdateUserRoleAsync(Guid userId, string newRole)
    {
        if (_currentUserService.Role != "Admin")
            throw new UnauthorizedAccessException("Admin access required");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        user.Role = newRole;
        await _userRepository.UpdateAsync(user);
    }
}
