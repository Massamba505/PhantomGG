using PhantomGG.API.Services.Interfaces;

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
        var invalidMessage = "Invalid or expired token.";

        string? headerToken = GetTokenFromHeader(context);
        string? cookieToken = GetTokenFromCookie(context);

        if (string.IsNullOrEmpty(headerToken) && string.IsNullOrEmpty(cookieToken))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(invalidMessage);
            return;
        }

        if (!string.IsNullOrEmpty(headerToken) && !string.IsNullOrEmpty(cookieToken))
        {
            if (!string.Equals(headerToken, cookieToken, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(invalidMessage);
                return;
            }
        }

        var principal = tokenService.ValidateToken(headerToken!);
        if (principal == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(invalidMessage);
            return;
        }

        context.User = principal;
        await _next(context);
    }

    private string? GetTokenFromHeader(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }

    private string? GetTokenFromCookie(HttpContext context)
    {
        context.Request.Cookies.TryGetValue("accessToken", out var token);
        return token;
    }
}