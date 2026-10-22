using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Content.Infrastructure.Persistence;

internal sealed class DbInitializer(
    ContentDbContext dbContext,
    ILogger<DbInitializer> logger)
    : IDbInitializer
{
    private readonly ContentDbContext _dbContext = dbContext;
    private readonly ILogger<DbInitializer> _logger = logger;

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Starting database migration");
        await _dbContext.Database.MigrateAsync();
        _logger.LogInformation("Database migration completed");
    }
}
