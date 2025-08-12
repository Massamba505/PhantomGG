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

public class TokenManager(
    ApplicationDbContext context,
    JwtConfig jwtConfig,
    Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager
    ) : ITokenManager
{
    private readonly ApplicationDbContext _context = context;
    private readonly JwtConfig _jwtConfig = jwtConfig;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = userManager;

    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user)
    {
        var accessToken = GenerateAccessToken(user);
        
        var refreshToken = GenerateRefreshToken(user);
        
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

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var storedToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == token);
            
        if (storedToken == null)
        {
            return false;
        }
        
        storedToken.RevokedAt = DateTime.UtcNow;
        
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();
        
        return true;
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

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

    private RefreshToken GenerateRefreshToken(ApplicationUser user)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);
        
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        return refreshTokenEntity;
    }
}
