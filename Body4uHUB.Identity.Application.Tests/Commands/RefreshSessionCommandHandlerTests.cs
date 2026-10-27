using Body4uHUB.Identity.Application.Commands.RefreshSession;
using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Identity.Application.Tests.Fakes;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class RefreshSessionCommandHandlerTests
    {
        private const string Email = "test@mail.com";
        private const string AccessToken = "access-token";

        private static readonly DateTime SignedInAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan Inactivity = TimeSpan.FromDays(7);
        private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);

        private Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IRoleRepository> _roleRepository;
        private Mock<IJwtTokenService> _jwtTokenService;
        private Mock<IUnitOfWork> _unitOfWork;
        private User _user;
        private RefreshToken _token;
        private string _raw;
        private RefreshToken _added;

        [SetUp]
        public void Setup()
        {
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _userRepository = new Mock<IUserRepository>();
            _roleRepository = new Mock<IRoleRepository>();
            _jwtTokenService = new Mock<IJwtTokenService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _added = null;

            _user = User.Create("hash", "Test", "User", Email, "0884787878", SignedInAt);
            _token = RefreshToken.Issue(_user.Id, true, SignedInAt, Inactivity, Lifetime, out _raw);

            // Looks up whichever token currently holds the hash, so chained rotations work.
            _refreshTokenRepository
                .Setup(x => x.GetByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string hash, CancellationToken _) => Known().FirstOrDefault(x => x.TokenHash == hash));
            _refreshTokenRepository
                .Setup(x => x.Add(It.IsAny<RefreshToken>()))
                .Callback<RefreshToken>(x => _added = x);

            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_user);

            _roleRepository
                .Setup(x => x.FindByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _jwtTokenService
                .Setup(x => x.GenerateAccessToken(_user.Id, Email, It.IsAny<IReadOnlyCollection<Role>>()))
                .Returns(AccessToken);
        }

        private IEnumerable<RefreshToken> Known() => new[] { _token, _added }.Where(x => x != null);

        private Task<Result<AuthSession>> Send(string token, DateTime now)
        {
            var handler = new RefreshSessionCommandHandler(
                _refreshTokenRepository.Object,
                _userRepository.Object,
                new AuthSessionFactory(_roleRepository.Object, _jwtTokenService.Object),
                _unitOfWork.Object,
                Options.Create(new RefreshTokenSettings()),
                new FixedTimeProvider(new DateTimeOffset(now, TimeSpan.Zero)));

            return handler.Handle(new RefreshSessionCommand(token), CancellationToken.None);
        }

        [Test]
        public async Task Handle_ShouldRotateTokenAndIssueAccessToken_WhenTokenIsActive()
        {
            var now = SignedInAt.AddDays(2);

            var result = await Send(_raw, now);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Response.AccessToken, Is.EqualTo(AccessToken));
            Assert.That(result.Value.Response.SessionExpiresAt, Is.EqualTo(SignedInAt.Add(Lifetime)));
            Assert.That(result.Value.RefreshToken, Is.Not.EqualTo(_raw));
            Assert.That(result.Value.RefreshTokenExpiresAt, Is.EqualTo(now.Add(Inactivity)));
            Assert.That(result.Value.IsPersistent, Is.True);
            Assert.That(_token.RevokedAt, Is.EqualTo(now));
            Assert.That(_token.ReplacedByTokenId, Is.EqualTo(_added.Id));
            Assert.That(_added.FamilyId, Is.EqualTo(_token.FamilyId));
            Assert.That(_added.TokenHash, Is.EqualTo(RefreshToken.HashToken(result.Value.RefreshToken)));
            _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldIssueAccessTokenWithCurrentRoles()
        {
            var admin = Role.Create("Administrator");
            _user.AddRole(admin);
            _roleRepository
                .Setup(x => x.FindByIdsAsync(It.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(admin.Id)), It.IsAny<CancellationToken>()))
                .ReturnsAsync([admin]);

            var result = await Send(_raw, SignedInAt.AddMinutes(20));

            Assert.That(result.Value.Response.User.Roles.Select(x => x.Name), Is.EqualTo(new[] { "Administrator" }));
            _jwtTokenService.Verify(
                x => x.GenerateAccessToken(_user.Id, Email, It.Is<IReadOnlyCollection<Role>>(roles => roles.Single() == admin)),
                Times.Once);
        }

        [Test]
        public async Task Handle_ShouldNotSlidePastTheSessionLifetime()
        {
            // A refresh every 6 days slides the expiry each time, but never beyond sign-in + 30 days.
            var raw = _raw;
            var now = SignedInAt;
            for (var i = 0; i < 4; i++)
            {
                now = now.AddDays(6);
                raw = (await Send(raw, now)).Value.RefreshToken;
                _token = _added;
                _added = null;
            }

            var last = await Send(raw, now.AddDays(5)); // day 29

            Assert.That(last.IsSuccess, Is.True);
            Assert.That(last.Value.RefreshTokenExpiresAt, Is.EqualTo(SignedInAt.Add(Lifetime)));
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            var result = await Send(null, SignedInAt);

            AssertUnauthorized(result);
            _refreshTokenRepository.Verify(x => x.GetByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenTokenIsUnknown()
        {
            AssertUnauthorized(await Send("unknown", SignedInAt));
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorized_WhenTokenExpiredFromInactivity()
        {
            AssertUnauthorized(await Send(_raw, SignedInAt.Add(Inactivity).AddSeconds(1)));
            Assert.That(_added, Is.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorizedWithoutEndingTheFamily_WhenTokenWasRevokedByLogout()
        {
            _token.Revoke(SignedInAt.AddMinutes(1));

            AssertUnauthorized(await Send(_raw, SignedInAt.AddMinutes(2)));
            _refreshTokenRepository.Verify(
                x => x.GetActiveByFamilyAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_ShouldEndTheWholeSession_WhenAnAlreadyRotatedTokenIsReused()
        {
            var now = SignedInAt.AddMinutes(20);
            var first = await Send(_raw, now);
            var successor = _added;
            _refreshTokenRepository
                .Setup(x => x.GetActiveByFamilyAsync(_token.FamilyId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([successor]);

            // The old cookie value again: a copy held by someone else.
            var reuse = await Send(_raw, now.AddMinutes(1));

            Assert.That(first.IsSuccess, Is.True);
            AssertUnauthorized(reuse);
            Assert.That(successor.RevokedAt, Is.Not.Null);
        }

        [Test]
        public async Task Handle_ShouldReturnUnauthorizedAndRevoke_WhenUserNoLongerExists()
        {
            _userRepository
                .Setup(x => x.GetByIdAsync(_user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            AssertUnauthorized(await Send(_raw, SignedInAt.AddMinutes(20)));
            Assert.That(_token.RevokedAt, Is.Not.Null);
            Assert.That(_added, Is.Null);
        }

        private static void AssertUnauthorized(Result<AuthSession> result)
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Unauthorized));
            Assert.That(result.Error, Is.EqualTo(SessionInvalid));
        }
    }
}
