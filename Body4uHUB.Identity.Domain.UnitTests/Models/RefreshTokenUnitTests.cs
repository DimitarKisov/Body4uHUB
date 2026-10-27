using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Domain.UnitTests.Models
{
    [TestFixture]
    public class RefreshTokenUnitTests
    {
        private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan Inactivity = TimeSpan.FromDays(7);
        private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);

        private static RefreshToken Issue(out string raw, bool persistent = false)
            => RefreshToken.Issue(Guid.NewGuid(), persistent, Now, Inactivity, Lifetime, out raw);

        [Test]
        public void Issue_ShouldStoreOnlyTheHashAndSetExpiries()
        {
            var token = Issue(out var raw, persistent: true);

            Assert.That(raw, Has.Length.GreaterThanOrEqualTo(43));
            Assert.That(token.TokenHash, Is.EqualTo(RefreshToken.HashToken(raw)));
            Assert.That(token.TokenHash, Is.Not.EqualTo(raw));
            Assert.That(token.TokenHash, Has.Length.EqualTo(TokenHashLength));
            Assert.That(token.ExpiresAt, Is.EqualTo(Now.Add(Inactivity)));
            Assert.That(token.FamilyExpiresAt, Is.EqualTo(Now.Add(Lifetime)));
            Assert.That(token.IsPersistent, Is.True);
            Assert.That(token.IsActive(Now), Is.True);
        }

        [Test]
        public void Issue_ShouldStartANewFamilyWithAUniqueValueEachTime()
        {
            var first = Issue(out var firstRaw);
            var second = Issue(out var secondRaw);

            Assert.That(firstRaw, Is.Not.EqualTo(secondRaw));
            Assert.That(first.FamilyId, Is.Not.EqualTo(second.FamilyId));
        }

        [Test]
        public void Issue_ShouldThrow_WhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidRefreshTokenException>(() => RefreshToken.Issue(Guid.Empty, false, Now, Inactivity, Lifetime, out _));

            Assert.That(ex.Error, Is.EqualTo(UserIdRequired));
        }

        [Test]
        public void Issue_ShouldThrow_WhenTimeIsNotUtc()
        {
            Assert.Throws<ArgumentException>(() => RefreshToken.Issue(Guid.NewGuid(), false, DateTime.Now, Inactivity, Lifetime, out _));
        }

        [Test]
        public void Rotate_ShouldRevokeThisTokenAndReturnItsSuccessorInTheSameFamily()
        {
            var token = Issue(out var raw, persistent: true);
            var later = Now.AddDays(3);

            var next = token.Rotate(later, Inactivity, out var nextRaw);

            Assert.That(token.IsActive(later), Is.False);
            Assert.That(token.RevokedAt, Is.EqualTo(later));
            Assert.That(token.ReplacedByTokenId, Is.EqualTo(next.Id));
            Assert.That(token.WasRotated, Is.True);
            Assert.That(nextRaw, Is.Not.EqualTo(raw));
            Assert.That(next.TokenHash, Is.EqualTo(RefreshToken.HashToken(nextRaw)));
            Assert.That(next.FamilyId, Is.EqualTo(token.FamilyId));
            Assert.That(next.UserId, Is.EqualTo(token.UserId));
            Assert.That(next.IsPersistent, Is.True);
            Assert.That(next.ExpiresAt, Is.EqualTo(later.Add(Inactivity)));
            Assert.That(next.FamilyExpiresAt, Is.EqualTo(token.FamilyExpiresAt));
        }

        [Test]
        public void Rotate_ShouldNotSlidePastTheFamilyEnd()
        {
            var token = Issue(out _);
            var nearTheEnd = Now.Add(Lifetime).AddDays(-2);
            var renewed = token.Rotate(Now.AddDays(6), Inactivity, out _)
                .Rotate(Now.AddDays(12), Inactivity, out _)
                .Rotate(Now.AddDays(18), Inactivity, out _)
                .Rotate(Now.AddDays(24), Inactivity, out _);

            var last = renewed.Rotate(nearTheEnd, Inactivity, out _);

            Assert.That(last.ExpiresAt, Is.EqualTo(Now.Add(Lifetime)));
        }

        [Test]
        public void Rotate_ShouldThrow_WhenTokenIsExpired()
        {
            var token = Issue(out _);

            var ex = Assert.Throws<InvalidRefreshTokenException>(() => token.Rotate(Now.Add(Inactivity), Inactivity, out _));

            Assert.That(ex.Error, Is.EqualTo(RefreshTokenNotActive));
        }

        [Test]
        public void Rotate_ShouldThrow_WhenTokenWasAlreadyRotated()
        {
            var token = Issue(out _);
            token.Rotate(Now.AddMinutes(1), Inactivity, out _);

            Assert.Throws<InvalidRefreshTokenException>(() => token.Rotate(Now.AddMinutes(2), Inactivity, out _));
        }

        [Test]
        public void Revoke_ShouldEndTheTokenWithoutMarkingItRotated_AndKeepTheFirstRevocationTime()
        {
            var token = Issue(out _);

            token.Revoke(Now.AddMinutes(1));
            token.Revoke(Now.AddMinutes(5));

            Assert.That(token.IsActive(Now.AddMinutes(2)), Is.False);
            Assert.That(token.RevokedAt, Is.EqualTo(Now.AddMinutes(1)));
            Assert.That(token.WasRotated, Is.False);
        }

        [Test]
        public void IsActive_ShouldBeFalse_FromTheExpiryOn()
        {
            var token = Issue(out _);

            Assert.That(token.IsActive(token.ExpiresAt.AddTicks(-1)), Is.True);
            Assert.That(token.IsActive(token.ExpiresAt), Is.False);
        }
    }
}
