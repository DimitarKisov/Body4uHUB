using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using Body4uHUB.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories;

internal sealed class UnitOfWork(ContentDbContext dbContext)
    : IUnitOfWork
{
    private readonly ContentDbContext _dbContext = dbContext;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new UniqueConstraintViolationException(ex);
        }
    }
}
