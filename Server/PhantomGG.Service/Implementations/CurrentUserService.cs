using PhantomGG.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Service.Exceptions;
using System.Security.Claims;
using PhantomGG.Common.Enums;

namespace PhantomGG.Service.Implementations;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUserDto GetCurrentUser()
    {
        if (!IsAuthenticated())
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var user = _httpContextAccessor.HttpContext?.User!;

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        var emailClaim = user.FindFirst(ClaimTypes.Email);
        var roleClaim = user.FindFirst(ClaimTypes.Role);

        if (idClaim == null || emailClaim == null || roleClaim == null)
        {
            throw new UnauthorizedException("Required claims are missing");
        }

        if (!Enum.TryParse<UserRoles>(roleClaim.Value, ignoreCase: true, out var role))
        {
            throw new UnauthorizedException("Invalid role claim");
        }

        return new CurrentUserDto
        {
            Id = Guid.Parse(idClaim.Value),
            Email = emailClaim.Value,
            Role = role
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
