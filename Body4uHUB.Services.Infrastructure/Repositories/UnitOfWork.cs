using Body4uHUB.Services.Infrastructure.Persistence;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Services.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ServicesDbContext _dbContext;

        public UnitOfWork(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
