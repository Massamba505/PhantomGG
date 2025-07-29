using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PhantomGG.API.Config;
using PhantomGG.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace PhantomGG.API.Middleware;

 public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenService tokenService)
    {
        var token = context.Request.Cookies["accessToken"];
        
        if (!string.IsNullOrEmpty(token))
        {
            var principal = tokenService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
                
                // Attach user info to context for easy access
                context.Items["UserId"] = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                context.Items["UserEmail"] = principal.FindFirst(ClaimTypes.Email)?.Value;
                context.Items["UserRole"] = principal.FindFirst(ClaimTypes.Role)?.Value;
            }
        }

        await _next(context);
    }
}