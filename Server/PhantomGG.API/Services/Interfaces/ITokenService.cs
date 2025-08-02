using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using System.Security.Claims;

namespace PhantomGG.API.Services.Interfaces;

public interface ITokenService
{
    Task<TokenPair> GenerateAuthResponseAsync(User user);
    ClaimsPrincipal? ValidateToken(string token);
}