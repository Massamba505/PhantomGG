using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Services.Implementations;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor
    ) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId 
    { 
        get 
        {
            var userId = GetClaimValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return null;

            if (Guid.TryParse(userId, out Guid result))
                return result;
                
            return null;
        } 
    }

    public string? Email => GetClaimValue(ClaimTypes.Email);
    
    public string? Role => GetClaimValue(ClaimTypes.Role);
    
    public bool IsAuthenticated => UserId.HasValue;

    private string? GetClaimValue(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(claimType);
            
        return string.IsNullOrEmpty(value) ? null : value;
    }
}