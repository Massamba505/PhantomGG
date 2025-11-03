using Microsoft.AspNetCore.Mvc;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, title) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation error"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict occurred"),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
        };

        var logLevel = statusCode switch
        {
            >= HttpStatusCode.InternalServerError => LogLevel.Error,
            >= HttpStatusCode.BadRequest => LogLevel.Warning,
            _ => LogLevel.Information
        };


        _logger.Log(logLevel, exception,
            "HTTP {RequestMethod} {RequestPath} failed with {StatusCode}: {ExceptionMessage}",
            context.Request.Method,
            context.Request.Path.Value,
            (int)statusCode,
            exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}