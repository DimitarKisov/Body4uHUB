using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Base;
using System.Security.Cryptography;
using System.Text;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Domain.Models;

/// <summary>
/// One link in a sign-in session ("family"). Every refresh rotates it: this token is revoked and points to its
/// replacement, so a second use of an already rotated token means it was copied, and the whole family is revoked.
/// </summary>
/// <remarks>
/// Its own aggregate rather than a collection on <see cref="User"/>: rotation and reuse detection only touch
/// tokens, and loading a user should not load every session it ever had.
/// </remarks>
public class RefreshToken : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }
    public Guid FamilyId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime FamilyExpiresAt { get; private set; }
    public bool IsPersistent { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    //За EF Core
    private RefreshToken() { }

    private RefreshToken(Guid userId, string tokenHash, Guid familyId, DateTime expiresAt, DateTime familyExpiresAt, bool isPersistent)
        : base(Guid.NewGuid())
    {
        UserId = userId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        ExpiresAt = expiresAt;
        FamilyExpiresAt = familyExpiresAt;
        IsPersistent = isPersistent;
    }

    /// <summary>Starts a new session. <paramref name="token"/> is the raw value for the cookie; only its hash is stored.</summary>
    public static RefreshToken Issue(Guid userId, bool isPersistent, DateTime utcNow, TimeSpan inactivity, TimeSpan lifetime, out string token)
    {
        if (userId == Guid.Empty)
        {
            throw new InvalidRefreshTokenException(UserIdRequired);
        }

        if (inactivity <= TimeSpan.Zero || lifetime < inactivity)
        {
            throw new ArgumentException("Inactivity must be positive and not longer than the lifetime.", nameof(inactivity));
        }

        var now = EnsureUtc(utcNow);
        var familyExpiresAt = now.Add(lifetime);
        token = NewTokenValue();

        return new RefreshToken(userId, HashToken(token), Guid.NewGuid(), now.Add(inactivity), familyExpiresAt, isPersistent);
    }

    /// <summary>
    /// Replaces this token with a new one in the same family. The new expiry slides by <paramref name="inactivity"/>,
    /// but never past the family's absolute end.
    /// </summary>
    public RefreshToken Rotate(DateTime utcNow, TimeSpan inactivity, out string token)
    {
        var now = EnsureUtc(utcNow);
        if (!IsActive(now))
        {
            throw new InvalidRefreshTokenException(RefreshTokenNotActive);
        }

        token = NewTokenValue();
        var expiresAt = now.Add(inactivity) < FamilyExpiresAt ? now.Add(inactivity) : FamilyExpiresAt;
        var next = new RefreshToken(UserId, HashToken(token), FamilyId, expiresAt, FamilyExpiresAt, IsPersistent);

        RevokedAt = now;
        ReplacedByTokenId = next.Id;

        return next;
    }

    /// <summary>Ends this token; a no-op when it is already revoked.</summary>
    public void Revoke(DateTime utcNow)
    {
        RevokedAt ??= EnsureUtc(utcNow);
    }

    public bool IsActive(DateTime utcNow) => RevokedAt == null && ExpiresAt > EnsureUtc(utcNow);

    /// <summary>Revoked because it was rotated (not by logout or a password change): using it again is a reuse.</summary>
    public bool WasRotated => ReplacedByTokenId != null;

    public static string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    // 256 random bits, base64url, so it fits a cookie without encoding.
    private static string NewTokenValue()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    // The caller supplies "now", so a local time here is a programming error rather than invalid user input.
    private static DateTime EnsureUtc(DateTime utcNow)
    {
        if (utcNow.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The time must be in UTC.", nameof(utcNow));
        }

        return utcNow;
    }
}
