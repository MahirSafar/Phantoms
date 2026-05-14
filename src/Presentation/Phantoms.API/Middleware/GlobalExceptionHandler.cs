using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Phantoms.Application.Common.Exceptions;
using ValidationException = Phantoms.Application.Common.Exceptions.ValidationException;

namespace Phantoms.API.Middleware;

/// <summary>
/// Modern .NET 8+ global exception handler using IExceptionHandler.
/// Catches ALL unhandled exceptions and returns a clean, standardized JSON response
/// that matches the project's Result wrapper shape. The frontend NEVER sees raw stack traces.
/// 
/// Mapping:
///   ValidationException    → 400 Bad Request   (field-specific errors)
///   NotFoundException      → 404 Not Found
///   ForbiddenAccessException → 403 Forbidden
///   UnauthorizedAccessException → 401 Unauthorized
///   Everything else        → 500 Internal Server Error
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse
                {
                    Succeeded = false,
                    Message = validationEx.Message,
                    Errors = validationEx.Errors
                        .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"))
                        .ToList(),
                    ValidationErrors = validationEx.Errors
                }),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ErrorResponse
                {
                    Succeeded = false,
                    Message = notFoundEx.Message,
                    Errors = [notFoundEx.Message]
                }),

            ForbiddenAccessException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                new ErrorResponse
                {
                    Succeeded = false,
                    Message = forbiddenEx.Message,
                    Errors = [forbiddenEx.Message]
                }),

            UnauthorizedAccessException unauthorizedEx => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse
                {
                    Succeeded = false,
                    Message = unauthorizedEx.Message ?? "You are not authorized.",
                    Errors = [unauthorizedEx.Message ?? "You are not authorized."]
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Succeeded = false,
                    Message = "An unexpected error occurred. Please try again later.",
                    Errors = ["An internal server error occurred."]
                })
        };

        // Log the full exception for developers; only return sanitized response to clients
        if (exception is not ValidationException)
        {
            logger.LogError(exception,
                "Unhandled exception [{ExceptionType}]: {Message}",
                exception.GetType().Name,
                exception.Message);
        }
        else
        {
            logger.LogWarning("Validation failure: {Message}", exception.Message);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions),
            cancellationToken);

        return true; // Exception is handled; don't propagate
    }
}

/// <summary>
/// Standardized error response shape that matches the project's Result wrapper.
/// The frontend can always rely on this structure for error handling.
/// </summary>
internal sealed class ErrorResponse
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public object? Data { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];

    /// <summary>
    /// Only populated for validation errors — maps property names to their specific error messages.
    /// </summary>
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
