namespace employeeManagement
{
    public static class Middleware
    {
        public static RequestDelegate LoggingMiddleware(RequestDelegate next)
        {
            return async context =>
            {
                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                await next(context);
                Console.WriteLine($"Request: {context.Response.StatusCode}");
            };
        }

        public static RequestDelegate ExceptionHandlingMiddleware(RequestDelegate next)
        {
            return async context =>
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected error occurred.");
                }
            };
        }
    }
}
