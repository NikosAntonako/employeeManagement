using Backend.Common;

namespace Backend.Middleware;

/// <summary>
/// Middleware that handles unhandled exceptions by returning standardized JSON error responses.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            _logger.LogError(
                exception,
                "Unhandled exception for HTTP {Method} {Path}. Responding with {StatusCode}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                context.TraceIdentifier);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var detail = statusCode == StatusCodes.Status500InternalServerError
                ? null
                : exception.Message;

            await context.Response.WriteAsJsonAsync(
                new ApiResponse<object>(statusCode, detail: detail));
        }
    }
}

