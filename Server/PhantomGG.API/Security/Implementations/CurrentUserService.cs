using PhantomGG.API.DTOs.User;
using PhantomGG.API.Security.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Security.Implementations;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUserDto GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        var emailClaim =  user.FindFirst(ClaimTypes.Email);
        var roleClaim = user.FindFirst(ClaimTypes.Role);

        if (idClaim == null || emailClaim == null || roleClaim == null)
        {
            throw new UnauthorizedAccessException("Required claims are missing");
        }

        return new CurrentUserDto
        {
            Id = Guid.Parse(idClaim.Value),
            Email = emailClaim.Value,
            Role = roleClaim.Value
        };
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }

    public bool IsInRole(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return false;

        var roleClaim = user.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value == role;
    }
}
