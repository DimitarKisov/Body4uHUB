using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly ContentDbContext _dbContext;

        public UnitOfWork(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}