using Microsoft.AspNetCore.Mvc;

namespace Backend.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Title = statusCode switch
                {
                    StatusCodes.Status404NotFound => "Resource not found.",
                    StatusCodes.Status400BadRequest => "Bad request.",
                    _ => "An unexpected error occurred."
                },
                Detail = exception.Message,
                Status = statusCode,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request details
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Request {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        await _next(context);

        // Log response details
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Response {StatusCode}", context.Response.StatusCode);
        }
    }
}