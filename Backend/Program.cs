using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Override [ApiController] automatic model-state response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new
        {
            statusCode = StatusCodes.Status400BadRequest,
            message = "Validation failed",
            errors
        });
    };
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("AllowedOrigins is not configured.");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<Backend.Middleware.LoggingMiddleware>();
app.UseMiddleware<Backend.Middleware.ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.Run();
