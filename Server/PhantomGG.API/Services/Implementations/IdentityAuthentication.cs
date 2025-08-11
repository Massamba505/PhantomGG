using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class IdentityAuthentication(
    IUserManager userManager,
    ITokenManager tokenManager,
    IRoleManager roleManager
    ) : IIdentityAuthentication
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IRoleManager _roleManager = roleManager;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userManager.UserExistsAsync(request.Email))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email is already registered"
            };
        }

        var newUser = await _userManager.CreateUserAsync(request);
        
        var tokenResponse = await _tokenManager.GenerateTokensAsync(newUser);
        
        return new AuthResponse
        {
            Success = true,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            AccessTokenExpires = tokenResponse.AccessTokenExpires,
            RefreshTokenExpires = tokenResponse.RefreshTokenExpires,
            User = _userManager.MapToUserDto(newUser)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        if (!user.IsActive)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Account is disabled"
            };
        }

        var result = await _userManager.ValidateCredentialsAsync(request.Email, request.Password);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account is locked due to too many failed attempts. Try again later or reset your password."
                };
            }
            
            if (result.IsNotAllowed)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Login not allowed. Email verification may be required."
                };
            }
            
            if (result.RequiresTwoFactor)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Two-factor authentication required"
                };
            }
            
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var tokenResponse = await _tokenManager.GenerateTokensAsync(user);

        return new AuthResponse
        {
            Success = true,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            AccessTokenExpires = tokenResponse.AccessTokenExpires,
            RefreshTokenExpires = tokenResponse.RefreshTokenExpires,
            User = _userManager.MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _tokenManager.ValidateRefreshTokenAsync(refreshToken);
        if (storedToken == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }
        
        var user = await _userManager.GetUserByIdAsync(storedToken.UserId);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found"
            };
        }
        
        await _tokenManager.RevokeTokenAsync(refreshToken);
        
        var tokenResponse = await _tokenManager.GenerateTokensAsync(user);
        
        return new AuthResponse
        {
            Success = true,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            AccessTokenExpires = tokenResponse.AccessTokenExpires,
            RefreshTokenExpires = tokenResponse.RefreshTokenExpires,
            User = _userManager.MapToUserDto(user)
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }
        
        var result = await _tokenManager.RevokeTokenAsync(refreshToken);
        
        await _userManager.SignOutAsync();
        
        return result;
    }

    public async Task<UserProfileDto?> GetCurrentUserAsync(Guid userId)
    {
        return await _userManager.GetUserProfileAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return await _userManager.GetUserByIdAsync(userId);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.GetUserByEmailAsync(email);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.UserExistsAsync(email);
    }

    public async Task<bool> AddUserToRoleAsync(Guid userId, string roleName)
    {
        return await _roleManager.AddUserToRoleAsync(userId, roleName);
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        return await _roleManager.RemoveUserFromRoleAsync(userId, roleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(Guid userId)
    {
        return await _roleManager.GetUserRolesAsync(userId);
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
    {
        return await _roleManager.IsUserInRoleAsync(userId, roleName);
    }

    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user)
    {
        return await _tokenManager.GenerateTokensAsync(user);
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        return await _tokenManager.RevokeTokenAsync(token);
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        return await _tokenManager.ValidateRefreshTokenAsync(token);
    }
}
