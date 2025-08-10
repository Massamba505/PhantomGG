using Microsoft.AspNetCore.Identity;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.DTOs.User;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Services.Managers.Interfaces;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the identity authentication service using manager delegation
/// </summary>
public class IdentityAuthentication : IIdentityAuthentication
{
    private readonly IUserManager _userManager;
    private readonly ITokenManager _tokenManager;
    private readonly IRoleManager _roleManager;

    /// <summary>
    /// Initializes a new instance of the IdentityAuthentication service
    /// </summary>
    /// <param name="userManager">User manager</param>
    /// <param name="tokenManager">Token manager</param>
    /// <param name="roleManager">Role manager</param>
    public IdentityAuthentication(
        IUserManager userManager,
        ITokenManager tokenManager,
        IRoleManager roleManager)
    {
        _userManager = userManager;
        _tokenManager = tokenManager;
        _roleManager = roleManager;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ipAddress = null)
    {
        // Check if email already exists
        if (await _userManager.UserExistsAsync(request.Email))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email is already registered"
            };
        }

        try
        {
            // Create user
            var newUser = await _userManager.CreateUserAsync(request);
            
            // Generate tokens
            var tokenResponse = await _tokenManager.GenerateTokensAsync(newUser, ipAddress);
            
            // Return response
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
        catch (InvalidOperationException ex)
        {
            return new AuthResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress = null)
    {
        // Find user by email
        var user = await _userManager.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Account is disabled"
            };
        }

        // Verify password
        var result = await _userManager.ValidateCredentialsAsync(request.Email, request.Password);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Account is locked. Try again later."
                };
            }
            
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Generate tokens
        var tokenResponse = await _tokenManager.GenerateTokensAsync(user, ipAddress);

        // Return response
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

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        // Validate the refresh token
        var storedToken = await _tokenManager.ValidateRefreshTokenAsync(refreshToken);
        if (storedToken == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }
        
        // Get user from the token
        var user = await _userManager.GetUserByIdAsync(storedToken.UserId);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found"
            };
        }
        
        // Revoke the current refresh token
        await _tokenManager.RevokeTokenAsync(refreshToken, ipAddress, "Replaced by new token");
        
        // Generate new tokens
        var tokenResponse = await _tokenManager.GenerateTokensAsync(user, ipAddress);
        
        // Return response
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

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string refreshToken, string? ipAddress = null)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }
        
        // Revoke the token
        var result = await _tokenManager.RevokeTokenAsync(refreshToken, ipAddress, "User logout");
        
        // Sign out from identity
        await _userManager.SignOutAsync();
        
        return result;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetCurrentUserAsync(string userId)
    {
        return await _userManager.GetUserProfileAsync(userId);
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.GetUserByIdAsync(userId);
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.GetUserByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userManager.UserExistsAsync(email);
    }

    /// <inheritdoc />
    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        return await _roleManager.AddUserToRoleAsync(userId, roleName);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        return await _roleManager.RemoveUserFromRoleAsync(userId, roleName);
    }

    /// <inheritdoc />
    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        return await _roleManager.GetUserRolesAsync(userId);
    }

    /// <inheritdoc />
    public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
    {
        return await _roleManager.IsUserInRoleAsync(userId, roleName);
    }

    /// <inheritdoc />
    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null)
    {
        return await _tokenManager.GenerateTokensAsync(user, ipAddress);
    }

    /// <inheritdoc />
    public async Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null)
    {
        return await _tokenManager.RevokeTokenAsync(token, ipAddress, reason, replacedByToken);
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        return await _tokenManager.ValidateRefreshTokenAsync(token);
    }
}
