using PhantomGG.API.Services.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Guid? UserId
    {
        get
        {
            var userId = GetClaimValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var id))
                return id;
            return null;
        }
    }

    public string Email => GetClaimValue(ClaimTypes.Email);
    public string Role => GetClaimValue(ClaimTypes.Role);
    public bool IsAuthenticated => UserId.HasValue;

    private string GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(claimType) ?? string.Empty;
    }
}