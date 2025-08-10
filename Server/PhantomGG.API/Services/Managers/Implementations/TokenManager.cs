using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhantomGG.API.Config;
using PhantomGG.API.Data;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Managers.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhantomGG.API.Services.Managers.Implementations;

/// <summary>
/// Implementation of the token manager
/// </summary>
public class TokenManager : ITokenManager
{
    private readonly ApplicationDbContext _context;
    private readonly JwtConfig _jwtConfig;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the TokenManager
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="jwtConfig">JWT configuration</param>
    /// <param name="userManager">Identity user manager</param>
    public TokenManager(
        ApplicationDbContext context,
        JwtConfig jwtConfig,
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _jwtConfig = jwtConfig;
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user, string? ipAddress = null)
    {
        // Generate access token
        var accessToken = GenerateAccessToken(user);
        
        // Generate refresh token
        var refreshToken = GenerateRefreshToken(user, ipAddress);
        
        // Save refresh token to database
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiryMinutes),
            RefreshTokenExpires = refreshToken.ExpiresAt
        };
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == token);
            
        if (storedToken == null || !storedToken.IsActive)
        {
            return null;
        }
        
        return storedToken;
    }

    /// <inheritdoc />
    public async Task<bool> RevokeTokenAsync(string token, string? ipAddress = null, string? reason = null, string? replacedByToken = null)
    {
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == token);
            
        if (storedToken == null)
        {
            return false;
        }
        
        // Revoke token
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        storedToken.ReasonRevoked = reason;
        storedToken.ReplacedByToken = replacedByToken;
        
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Generates a JWT access token
    /// </summary>
    /// <param name="user">Application user</param>
    /// <returns>JWT token string</returns>
    private string GenerateAccessToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        };

        // Add roles as claims
        var userRoles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpiryMinutes),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <param name="user">Application user</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <returns>Refresh token</returns>
    private RefreshToken GenerateRefreshToken(ApplicationUser user, string? ipAddress = null)
    {
        // Generate a secure random token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);
        
        // Create the refresh token entity
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        return refreshTokenEntity;
    }
}
