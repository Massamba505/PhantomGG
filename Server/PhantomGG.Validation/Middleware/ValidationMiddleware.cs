using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace PhantomGG.Validation.Middleware;

public class ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ValidationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        _logger.LogWarning("Validation failed for {RequestMethod} {RequestPath}: {ValidationErrors}",
            context.Request.Method,
            context.Request.Path.Value,
            string.Join(", ", errors.SelectMany(kv => kv.Value.Select(v => $"{kv.Key}: {v}"))));

        var problemDetails = new ValidationProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "One or more validation errors occurred.",
            Instance = context.Request.Path,
        };

        foreach (var error in errors)
        {
            problemDetails.Errors.Add(error.Key, error.Value);
        }

        var jsonResponse = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

}
