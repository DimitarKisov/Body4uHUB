using Body4uHUB.Identity.Api.Extensions;
using Body4uHUB.Identity.Api.Middleware;
using Body4uHUB.Identity.Application.Extensions;
using Body4uHUB.Identity.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("logs/identity-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var services = builder.Services;
var configuration = builder.Configuration;

services
       .AddApplication(configuration)
       .AddInfrastructure(configuration)
       .AddControllers();

services
    .AddEndpointsApiExplorer()
    .ConfigureSwagger();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Body4uHUB.Identity.Api");

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
