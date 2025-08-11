using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhantomGG.API.Config;
using PhantomGG.API.Data;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Services.Managers.Implementations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhantomGG.API.Services.Implementations;

public class TokenService(
    JwtConfig jwtConfig,
    ApplicationDbContext context,
    RoleManager roleManager
    ) : ITokenService
{
    private readonly JwtConfig _jwtConfig = jwtConfig;
    private readonly ApplicationDbContext _context = context;
    private readonly RoleManager _roleManager = roleManager;

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
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

        var userRoles = await _roleManager.GetUserRolesAsync(user.Id);

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

    public RefreshToken GenerateRefreshToken(ApplicationUser user)
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
            CreatedAt = DateTime.UtcNow,
        };

        return refreshTokenEntity;
    }

    public async Task<TokenResponse> GenerateTokensAsync(ApplicationUser user)
    {
        var accessToken = await GenerateAccessTokenAsync(user);
        
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

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await ValidateRefreshTokenAsync(refreshToken);
        if (storedToken == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }
        
        var user = await _context.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }
        
        await RevokeTokenAsync(refreshToken);
        
        return await GenerateTokensAsync(user);
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