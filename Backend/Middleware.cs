namespace Backend
{
    public static class Middleware
    {
        public static RequestDelegate ExceptionHandlingMiddleware(RequestDelegate next)
        {
            return async context =>
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
            };
        }

        public static RequestDelegate LoggingMiddleware(RequestDelegate next)
        {
            return async context =>
            {
                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                await next(context);
                Console.WriteLine($"Request: {context.Response.StatusCode}");
            };
        }
    }
}
