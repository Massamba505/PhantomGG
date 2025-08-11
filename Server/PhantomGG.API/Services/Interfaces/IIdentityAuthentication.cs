using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IIdentityAuthentication
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    
    Task<AuthResponse> LoginAsync(LoginRequest request);
    
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    
    Task<bool> LogoutAsync(string refreshToken);
    
    Task<UserProfileDto?> GetCurrentUserAsync(Guid userId);
    
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    
    Task<bool> UserExistsAsync(string email);
    
    Task<bool> AddUserToRoleAsync(Guid userId, string roleName);
    
    Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName);
    
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);
    
    Task<bool> RevokeTokenAsync(string token, string? reason = null, string? replacedByToken = null);
    
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);
}
