using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal sealed class RefreshTokenRepository(IdentityDbContext dbContext) : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _dbContext = dbContext;

        public void Add(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);
        }

        public async Task<RefreshToken> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByFamilyAsync(Guid familyId, DateTime utcNow, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens
                .Where(x => x.FamilyId == familyId && x.RevokedAt == null && x.ExpiresAt > utcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId, DateTime utcNow, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > utcNow)
                .ToListAsync(cancellationToken);
        }
    }
}
