namespace Backend.Middleware;

/// <summary>
/// Middleware that logs HTTP request and response information as part of the ASP.NET Core request pipeline.
/// </summary>
/// <remarks>This middleware logs the HTTP method and path of incoming requests, as well as the status code of
/// outgoing responses, when information-level logging is enabled. It should be registered early in the pipeline to
/// capture all relevant request and response data.</remarks>
/// <param name="next">The next middleware delegate in the request pipeline. Cannot be null.</param>
/// <param name="logger">The logger used to record request and response information. Cannot be null.</param>
public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Request {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        await _next(context);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Response {StatusCode}", context.Response.StatusCode);
        }
    }
}