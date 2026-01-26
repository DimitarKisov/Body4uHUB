using Body4uHUB.Services.Api.Extensions;
using Body4uHUB.Services.Api.Middleware;
using Body4uHUB.Services.Application.Extensions;
using Body4uHUB.Services.Infrastructure.Extensions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Serilog;

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
    .AddApiServices(configuration)
    .AddApplication(configuration)
    .AddInfrastructure(configuration)
    .AddHealthChecks();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
await dbInitializer.InitializeAsync();

try
{
    Log.Information("Starting Body4uHUB.Services.Api");
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