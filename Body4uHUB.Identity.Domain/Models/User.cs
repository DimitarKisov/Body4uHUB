using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;
using System.Security.Cryptography;
using System.Text;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Domain.Models;

public class User : AggregateRoot<Guid>
{
    private readonly List<UserRole> _roles = new();

    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string EmailConfirmationToken { get; private set; }
    public DateTime? EmailConfirmationTokenExpiry { get; private set; }
    public string PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    //За EF Core
    private User() { }

    internal User(Guid id, string passwordHash, string firstName, string lastName, ContactInfo contactInfo, DateTime utcNow)
        : base(id)
    {
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        ContactInfo = contactInfo;
        GenerateEmailConfirmationToken(utcNow);
    }

    public static User Create(string passwordHash, string firstName, string lastName, string email, string phoneNumber, DateTime utcNow)
    {
        Validate(passwordHash, firstName, lastName);

        var contactInfo = ContactInfo.Create(email, phoneNumber);

        var user = new User(Guid.NewGuid(), passwordHash, firstName, lastName, contactInfo, utcNow);
        user.AddDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }

    public void AddRole(Role role)
    {
        Guard.AgainstDefault<InvalidUserException, Role>(role, nameof(role));

        // The Trainer role goes only through AssignTrainerRole, together with the trainer account.
        if (role.Name == TrainerRoleName)
        {
            throw new InvalidUserException(TrainerRoleRequiresTrainerAccount);
        }

        if (HasRole(role.Id))
        {
            return;
        }

        _roles.Add(new UserRole(role.Id));
    }

    public void UpdateFirstName(string firstName)
    {
        ValidateFirstName(firstName);
        FirstName = firstName;
    }

    public void UpdateLastName(string lastName)
    {
        ValidateLastName(lastName);
        LastName = lastName;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
    }

    public void UpdateContactInfo(string email, string phoneNumber)
    {
        ContactInfo = ContactInfo.Create(email, phoneNumber);
    }

    public void UpdateLastLogin(DateTime utcNow)
    {
        LastLoginAt = EnsureUtc(utcNow);
    }

    public void ConfirmEmail(string token, DateTime utcNow)
    {
        var tokenMatches = !string.IsNullOrWhiteSpace(token)
            && string.Equals(token, EmailConfirmationToken, StringComparison.Ordinal);

        // Repeating a successful confirmation with the same token is a no-op. The token is kept so that
        // any other token fails the same way for confirmed and unconfirmed users alike.
        if (IsEmailConfirmed && tokenMatches)
        {
            return;
        }

        var isTokenExpired = !(EmailConfirmationTokenExpiry > EnsureUtc(utcNow));

        if (IsEmailConfirmed || !tokenMatches || isTokenExpired)
        {
            throw new InvalidUserException(EmailConfirmationTokenInvalid);
        }

        IsEmailConfirmed = true;
    }

    public void RegenerateEmailConfirmationToken(DateTime utcNow)
    {
        // A confirmed user keeps the token that confirmed them, so ConfirmEmail with it stays a no-op.
        if (IsEmailConfirmed)
        {
            return;
        }

        GenerateEmailConfirmationToken(utcNow);
        AddDomainEvent(new EmailConfirmationTokenRegeneratedDomainEvent(Id));
    }

    // Only the SHA-256 hash of the token is stored, so a leaked database cannot be used to reset passwords.
    // A new request replaces the previous token, so only the latest email link works.
    public void RequestPasswordReset(DateTime utcNow)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        PasswordResetTokenHash = HashPasswordResetToken(token);
        PasswordResetTokenExpiry = EnsureUtc(utcNow).AddHours(PasswordResetTokenLifetimeHours);
        AddDomainEvent(new PasswordResetRequestedDomainEvent(Id, token));
    }

    // The token is single-use: a successful reset clears it, so the same link cannot be used twice.
    public void ResetPassword(string token, string newPasswordHash, DateTime utcNow)
    {
        var tokenMatches = !string.IsNullOrWhiteSpace(token)
            && PasswordResetTokenHash != null
            && CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(HashPasswordResetToken(token)),
                Encoding.ASCII.GetBytes(PasswordResetTokenHash));

        var isTokenExpired = !(PasswordResetTokenExpiry > EnsureUtc(utcNow));

        if (!tokenMatches || isTokenExpired)
        {
            throw new InvalidUserException(PasswordResetTokenInvalid);
        }

        UpdatePasswordHash(newPasswordHash);
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiry = null;
    }

    public bool HasRole(Guid roleId) => _roles.Any(x => x.RoleId == roleId);

    // The Trainer role and the trainer account go together: the role is granted when the account is
    // created and revoked when it is deleted, so a user cannot keep trainer access without a profile.
    public void AssignTrainerRole(Role trainerRole)
    {
        ValidateTrainerRole(trainerRole);

        if (HasRole(trainerRole.Id))
        {
            throw new InvalidUserException(UserAlreadyTrainer);
        }

        _roles.Add(new UserRole(trainerRole.Id));
        AddDomainEvent(new TrainerRoleAssignedDomainEvent(Id));
    }

    public void RevokeTrainerRole(Role trainerRole)
    {
        ValidateTrainerRole(trainerRole);

        if (_roles.RemoveAll(x => x.RoleId == trainerRole.Id) == 0)
        {
            throw new InvalidUserException(UserNotInRole);
        }

        AddDomainEvent(new TrainerRoleRevokedDomainEvent(Id));
    }

    private static void ValidateTrainerRole(Role trainerRole)
    {
        Guard.AgainstDefault<InvalidUserException, Role>(trainerRole, nameof(trainerRole));

        if (trainerRole.Name != TrainerRoleName)
        {
            throw new InvalidUserException(RoleIsNotTrainer);
        }
    }

    private void GenerateEmailConfirmationToken(DateTime utcNow)
    {
        EmailConfirmationToken = Guid.NewGuid().ToString();
        EmailConfirmationTokenExpiry = EnsureUtc(utcNow).AddHours(EmailConfirmationTokenLifetimeHours);
    }

    private static string HashPasswordResetToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    // The caller supplies "now", so a local time here is a programming error rather than invalid user input.
    private static DateTime EnsureUtc(DateTime utcNow)
    {
        if (utcNow.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The time must be in UTC.", nameof(utcNow));
        }

        return utcNow;
    }

    private static void Validate(string passwordHash, string firstName, string lastName)
    {
        ValidatePasswordHash(passwordHash);
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
    }

    private static void ValidateFirstName(string firstName)
    {
        Guard.AgainstEmptyString<InvalidUserException>(firstName, nameof(firstName));
        Guard.ForStringLength<InvalidUserException>(firstName, MinNameLength, MaxNameLength, nameof(firstName));
    }

    private static void ValidateLastName(string lastName)
    {
        Guard.AgainstEmptyString<InvalidUserException>(lastName, nameof(lastName));
        Guard.ForStringLength<InvalidUserException>(lastName, MinNameLength, MaxNameLength, nameof(lastName));
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        Guard.AgainstEmptyString<InvalidUserException>(passwordHash, nameof(passwordHash));
    }
}
