namespace Body4uHUB.Content.Infrastructure.Repositories
{
    using Body4uHUB.Content.Infrastructure.Persistence;
    using Body4uHUB.Shared;

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