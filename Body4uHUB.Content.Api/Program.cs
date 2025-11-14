using Body4uHUB.Content.Api.Middleware;
using Body4uHUB.Content.Application.Extensions;
using Body4uHUB.Content.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("logs/content-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var services = builder.Services;
var configuration = builder.Configuration;

// Add layers
services
    .AddApplication(configuration)
    .AddInfrastructure(configuration)
    .AddControllers();

services.AddEndpointsApiExplorer();

// Add authorization policies
services.AddAuthorizationBuilder()
    .AddPolicy("TrainerOrAdmin", policy =>
        policy.RequireRole("Trainer", "Admin"))
    .AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Body4uHUB Content & Community API",
        Version = "v1",
        Description = "API за управление на статии, форуми, коментари и bookmarks"
    });

    config.CustomSchemaIds(x => x.FullName);

    // JWT Bearer Authentication
    config.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Body4uHUB.Content.Api");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}