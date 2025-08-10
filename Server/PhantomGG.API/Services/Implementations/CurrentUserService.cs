using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Services.Implementations;

/// <summary>
/// Implementation of the current user service
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the CurrentUserService
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string? UserId => GetClaimValue(ClaimTypes.NameIdentifier);

    /// <inheritdoc />
    public string? Email => GetClaimValue(ClaimTypes.Email);
    
    /// <inheritdoc />
    public string? Role => GetClaimValue(ClaimTypes.Role);
    
    /// <inheritdoc />
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

    private string? GetClaimValue(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(claimType);
            
        return string.IsNullOrEmpty(value) ? null : value;
    }
}