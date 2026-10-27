using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Identity.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByFamilyAsync(Guid familyId, DateTime utcNow, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId, DateTime utcNow, CancellationToken cancellationToken = default);
}
