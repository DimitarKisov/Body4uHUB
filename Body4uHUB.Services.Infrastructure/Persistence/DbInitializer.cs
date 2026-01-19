using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Services.Infrastructure.Persistence
{
    internal class DbInitializer : IDbInitializer
    {
        private readonly ServicesDbContext _dbContext;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ServicesDbContext dbContext,
            ILogger<DbInitializer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Starting database migration");
            await _dbContext.Database.MigrateAsync();
            _logger.LogInformation("Database migration completed");
        }
    }
}
