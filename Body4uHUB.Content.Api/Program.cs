using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Api.Middleware;
using Body4uHUB.Content.Application.Extensions;
using Body4uHUB.Content.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.ConfigureSerilog();

var services = builder.Services;
var configuration = builder.Configuration;

// Add services
services
    .AddApiServices()
    .AddApplication(configuration)
    .AddInfrastructure(configuration)
    .AddAuthorizationPolicies()
    .ConfigureSwagger();

var app = builder.Build();

// Middleware pipeline
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