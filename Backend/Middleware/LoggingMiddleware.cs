using System.Diagnostics;

namespace Backend.Middleware;

/// <summary>
/// Middleware that logs HTTP request/response details for each request.
/// </summary>
public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var traceId = context.TraceIdentifier;
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms. TraceId: {TraceId}",
                method,
                path,
                statusCode,
                elapsedMs,
                traceId);
        }
        else if (statusCode >= StatusCodes.Status400BadRequest)
        {
            _logger.LogWarning(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms. TraceId: {TraceId}",
                method,
                path,
                statusCode,
                elapsedMs,
                traceId);
        }
        else if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms. TraceId: {TraceId}",
                method,
                path,
                statusCode,
                elapsedMs,
                traceId);
        }
    }
}