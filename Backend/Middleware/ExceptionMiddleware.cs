using Backend.Common;

namespace Backend.Middleware;

/// <summary>
/// Middleware that handles unhandled exceptions by returning standardized JSON error responses with appropriate HTTP
/// status codes.
/// </summary>
/// <remarks>This middleware maps specific exception types to HTTP status codes: 404 for KeyNotFoundException, 400
/// for ArgumentException, and 500 for all other exceptions. For 500 errors, the response omits the exception message
/// for security reasons. Place this middleware early in the pipeline to ensure consistent error handling.</remarks>
/// <param name="next">The next middleware component in the request processing pipeline.</param>
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
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            // Pass exception message as detail, or null for 500 errors (security)
            var detail = context.Response.StatusCode == StatusCodes.Status500InternalServerError
                ? null
                : exception.Message;

            await context.Response.WriteAsJsonAsync(
                new ApiResponse<object>(context.Response.StatusCode, detail: detail));
        }
    }
}

