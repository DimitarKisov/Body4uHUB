using Body4uHUB.Identity.Domain.Repositories;

namespace Body4uHUB.Identity.Application.Services
{
    internal static class RefreshTokenRepositoryExtensions
    {
        /// <summary>
        /// Revokes every active session of the user (saved with the caller's unit of work), except the family of
        /// <paramref name="keepTokenHash"/> when given — the session that made the change.
        /// </summary>
        public static async Task RevokeAllForUserAsync(
            this IRefreshTokenRepository repository,
            Guid userId,
            DateTime utcNow,
            CancellationToken cancellationToken,
            string keepTokenHash = null)
        {
            var tokens = await repository.GetActiveByUserAsync(userId, utcNow, cancellationToken);
            var keepFamilyId = keepTokenHash == null ? null : tokens.FirstOrDefault(x => x.TokenHash == keepTokenHash)?.FamilyId;

            foreach (var token in tokens.Where(x => x.FamilyId != keepFamilyId))
            {
                token.Revoke(utcNow);
            }
        }
    }
}
