using PhantomGG.Models.DTOs;
using PhantomGG.Service.Exceptions;
using System.Net;
using System.Text.Json;

namespace PhantomGG.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

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
        var response = context.Response;
        response.ContentType = "application/json";
        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        var message = exception.Message;
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            message = "An internal server error occurred";
        }

        var apiResponse = new ApiResponse
        {
            Success = false,
            Message = message
        };

        response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }

}
