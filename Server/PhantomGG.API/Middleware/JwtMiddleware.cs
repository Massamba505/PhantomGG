using System.Security.Claims;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Utils;

namespace PhantomGG.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, JwtUtils jwtUtils, IUserRepository userRepo)
    {
        var token = GetTokenFromHeaderOrCookie(context);

        if (token != null)
        {
            if (!TokensMatch(context, token))
            {
                throw new UnauthorizedAccessException("Token mismatch - possible security violation");
            }

            var principal = jwtUtils.ValidateAccessToken(token);
            if (principal != null)
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userId, out var id))
                {
                    var user = await userRepo.GetByIdAsync(id);
                    if (user != null)
                    {
                        context.Items["User"] = user;
                    }
                }
            }
        }

        await _next(context);
    }

    private string? GetTokenFromHeaderOrCookie(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader[7..]; 
        }

        return context.Request.Cookies["accessToken"];
    }

    private bool TokensMatch(HttpContext context, string token)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        string? headerToken = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            headerToken = authHeader[7..];
        }

        var cookieToken = context.Request.Cookies["accessToken"];

        if (headerToken != null)
        {
            return headerToken == cookieToken;
        }

        return true;
    }
}