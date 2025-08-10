using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhantomGG.API.Config;
using PhantomGG.API.Data;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the token service
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TokenService
    /// </summary>
    /// <param name="jwtConfig">JWT configuration</param>
    /// <param name="context">Database context</param>
    public TokenService(JwtConfig jwtConfig, ApplicationDbContext context)
    {
        _jwtConfig = jwtConfig;
        _context = context;
    }

    /// <inheritdoc />
    public string GenerateAccessToken(ApplicationUser user)
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
        var userRoles = _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToList();

        foreach (var role in userRoles)
        {
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
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

    /// <inheritdoc />
    public RefreshToken GenerateRefreshToken(ApplicationUser user, string? ipAddress = null)
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
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        // Validate the refresh token
        var storedToken = await ValidateRefreshTokenAsync(refreshToken);
        if (storedToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }
        
        // Get user from the token
        var user = await _context.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }
        
        // Revoke the current refresh token
        await RevokeTokenAsync(refreshToken, ipAddress, "Replaced by new token");
        
        // Generate new tokens
        return await GenerateTokensAsync(user, ipAddress);
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
}