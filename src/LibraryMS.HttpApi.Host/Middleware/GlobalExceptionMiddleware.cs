using LibraryMS.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using LibraryMS.Application.Contracts.Common;

namespace LibraryMS.HttpApi.Host.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors, errorCode) = exception switch
        {
            FluentValidation.ValidationException ve => (400, "Validation Error", ve.Errors?.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage }) ?? new Dictionary<string, string[]>(), "VALIDATION_ERROR"),
            DomainException de => (400, de.Message, new Dictionary<string, string[]>(), "DOMAIN_ERROR"),
            NotFoundException ne => (404, ne.Message, new Dictionary<string, string[]>(), "NOT_FOUND"),
            UnauthorizedAccessException => (401, "Unauthorized", new Dictionary<string, string[]>(), "UNAUTHORIZED"),
            Microsoft.EntityFrameworkCore.DbUpdateException dbEx => (400, "Database Update Error: " + (dbEx.InnerException?.Message ?? dbEx.Message), new Dictionary<string, string[]>(), "DB_UPDATE_ERROR"),
            _ => (500, "Internal Server Error", new Dictionary<string, string[]>(), "INTERNAL_ERROR")
        };

        context.Response.StatusCode = statusCode;

        var apiResponse = ApiResponse<object>.FailureResult(message, errorCode, errors);

        var result = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(result);
    }
}
