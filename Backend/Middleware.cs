using Backend.Dtos;

namespace Backend;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"An unexpected error occurred in middleware. {exception.Message}");

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                ResponseModel<object>.Failure(exception.Message, new[] { exception.Message }));
        }
    }
}

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        await next(context);
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}
