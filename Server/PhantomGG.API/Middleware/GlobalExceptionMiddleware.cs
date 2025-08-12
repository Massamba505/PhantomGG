using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace PhantomGG.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, the global exception middleware will not be executed.");
            return; 
        }

        context.Response.Clear();
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            SecurityTokenException => (StatusCodes.Status401Unauthorized, "Invalid Token", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
            FormatException => (StatusCodes.Status400BadRequest, "Invalid Format", exception.Message),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred")
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        var options = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(problemDetails, options);

        await context.Response.WriteAsync(json);
    }
}
