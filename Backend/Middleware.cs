namespace Backend
{
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
                Console.WriteLine($"An unexpected error occurred in middleware. {exception.Message}");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"An unexpected error occurred in middleware. {exception.Message}");
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
}
