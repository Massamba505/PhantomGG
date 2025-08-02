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
        var bearer = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(bearer) && bearer.StartsWith("Bearer "))
        {
            return bearer.Split(" ").Last();
        }

        return context.Request.Cookies["access_token"];
    }
}