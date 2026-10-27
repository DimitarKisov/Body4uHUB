using Body4uHUB.Identity.Application.Commands.Logout;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Domain.Abstractions;
using Moq;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class LogoutCommandHandlerTests
    {
        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<IUnitOfWork> _unitOfWork;
        private LogoutCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _handler = new LogoutCommandHandler(_refreshTokenRepository.Object, _unitOfWork.Object, TimeProvider.System);
        }

        [Test]
        public async Task Handle_ShouldRevokeTheSessionFamily_WhenTokenIsKnown()
        {
            var token = RefreshToken.Issue(Guid.NewGuid(), false, DateTime.UtcNow, TimeSpan.FromDays(7), TimeSpan.FromDays(30), out var raw);
            _refreshTokenRepository
                .Setup(x => x.GetByHashAsync(RefreshToken.HashToken(raw), It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);
            _refreshTokenRepository
                .Setup(x => x.GetActiveByFamilyAsync(token.FamilyId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([token]);

            var result = await _handler.Handle(new LogoutCommand(raw), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(token.RevokedAt, Is.Not.Null);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("unknown")]
        public async Task Handle_ShouldSucceedWithoutChanges_WhenThereIsNoKnownSession(string raw)
        {
            var result = await _handler.Handle(new LogoutCommand(raw), CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
