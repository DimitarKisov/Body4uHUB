using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Fakes
{
    /// <summary>Refresh tokens for handler tests that end a user's sessions.</summary>
    internal static class Sessions
    {
        /// <summary>A repository where no user has active sessions, unless a test sets some up.</summary>
        public static Mock<IRefreshTokenRepository> EmptyRepository()
        {
            var repository = new Mock<IRefreshTokenRepository>();
            repository
                .Setup(x => x.GetActiveByUserAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);
            return repository;
        }

        public static RefreshToken Active(Guid userId)
            => RefreshToken.Issue(userId, false, DateTime.UtcNow, TimeSpan.FromDays(7), TimeSpan.FromDays(30), out _);

        public static void SetupActive(this Mock<IRefreshTokenRepository> repository, Guid userId, params RefreshToken[] tokens)
            => repository
                .Setup(x => x.GetActiveByUserAsync(userId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokens);
    }
}
