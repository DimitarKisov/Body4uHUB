using Body4uHUB.Content.Api.Extensions;
using Body4uHUB.Content.Api.Middleware;
using Body4uHUB.Content.Application.Extensions;
using Body4uHUB.Content.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("Body4uHUB");
#else
// THIS WILL BE FOR PRODUCTION
#endif

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
    .ConfigureSwagger()
    .AddHealthChecks();

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
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync();

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