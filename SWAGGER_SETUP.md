# Swagger Setup Instructions

## 1. Install NuGet Package
```
Install-Package Swashbuckle.AspNetCore
```

## 2. Register Swagger Services in Program.cs
```
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

## 3. Configure Swagger Middleware in Program.cs
```
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```
