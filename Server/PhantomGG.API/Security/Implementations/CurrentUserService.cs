using PhantomGG.API.DTOs.User;
using PhantomGG.API.Security.Interfaces;
using System.Security.Claims;

namespace PhantomGG.API.Security.Implementations;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUserDto GetCurrentUser()
    {
        if (!IsAuthenticated())
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var user = _httpContextAccessor.HttpContext?.User!;

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
        if (!IsAuthenticated()) return false;
        var user = _httpContextAccessor.HttpContext?.User!;

        var roleClaim = user.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value == role;
    }
}
