using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Shared.Domain;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _dbContext;

        public UnitOfWork(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
