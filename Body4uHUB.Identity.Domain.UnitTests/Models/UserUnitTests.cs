using Body4uHUB.Identity.Domain.Events;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Domain.UnitTests.Models
{
    [TestFixture]
    public class UserUnitTests
    {
        private User _user;

        private const string ValidPasswordHash = "AQAAAAEAACcQAAAAEDummyHashValue==";
        private const string ValidFirstName = "Test";
        private const string ValidLastName = "User";
        private const string ValidEmail = "test@mail.com";
        private const string ValidPhone = "0884787878";
        private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        [SetUp]
        public void Setup()
        {
            _user = User.Create(
                ValidPasswordHash,
                ValidFirstName,
                ValidLastName,
                ValidEmail,
                ValidPhone, Now);
        }

        [TestCase(ValidPasswordHash, ValidFirstName, ValidLastName, ValidEmail, ValidPhone)]
        [TestCase(ValidPasswordHash, ValidFirstName, ValidLastName, ValidEmail, "+359884787878")]
        public void Create_ShouldCreateUser_WhenAllParametersAreValid(
            string passwordHash,
            string firstName,
            string lastName,
            string email,
            string phoneNumber)
        {
            var result = User.Create(
                passwordHash,
                firstName,
                lastName,
                email,
                phoneNumber, Now);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PasswordHash, Is.EqualTo(passwordHash));
            Assert.That(result.FirstName, Is.EqualTo(firstName));
            Assert.That(result.LastName, Is.EqualTo(lastName));
            Assert.That(result.ContactInfo.Email, Is.EqualTo(email));
            Assert.That(result.ContactInfo.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(result.IsEmailConfirmed, Is.False);
            Assert.That(Guid.TryParse(result.EmailConfirmationToken, out _), Is.True);
            Assert.That(result.EmailConfirmationTokenExpiry, Is.Not.Null);
            Assert.That(result.EmailConfirmationTokenExpiry, Is.EqualTo(Now.AddHours(EmailConfirmationTokenLifetimeHours)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Create_ShouldThrowInvalidUserException_WhenPasswordHashIsNullOrWhiteSpace(string passwordHash)
        {
            var ex = Assert.Throws<InvalidUserException>(() =>
                User.Create(
                    passwordHash,
                    ValidFirstName,
                    ValidLastName,
                    ValidEmail,
                    ValidPhone, Now));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(passwordHash)} cannot be null or empty."));
        }

        [Test]
        public void AddRole_ShouldAddRole_WhenRoleIsValid()
        {
            var role = Role.Create("TestRole");

            _user.AddRole(role);

            Assert.That(_user.HasRole(role.Id), Is.True);
        }

        [Test]
        public void AddRole_ShouldThrowInvalidUserException_WhenRoleIsNull()
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.AddRole(null));

            Assert.That(ex.Error, Is.EqualTo($"role cannot be the default value."));
        }

        [Test]
        public void AddRole_ShouldNotAddDuplicateRole_WhenRoleAlreadyExists()
        {
            var role = Role.Create("TestRole");

            _user.AddRole(role);
            _user.AddRole(role);

            Assert.That(_user.Roles.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddRole_ShouldThrowInvalidUserException_WhenRoleIsTrainer()
        {
            var trainerRole = Role.Create(TrainerRoleName);

            var ex = Assert.Throws<InvalidUserException>(() => _user.AddRole(trainerRole));

            Assert.That(ex.Error, Is.EqualTo(TrainerRoleRequiresTrainerAccount));
            Assert.That(_user.HasRole(trainerRole.Id), Is.False);
        }

        [TestCase("John")]
        [TestCase("Su")]
        [TestCase("VeryLongLongLongName")]
        public void UpdateFirstName_ShouldUpdateFirstName_WhenFirstNameIsValid(string firstName)
        {
            _user.UpdateFirstName(firstName);

            Assert.That(_user.FirstName, Is.EqualTo(firstName));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateFirstName_ShouldThrowInvalidUserException_WhenFirstNameIsNullOrWhiteSpace(string firstName)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.UpdateFirstName(firstName));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(firstName)} cannot be null or empty."));
        }

        [TestCase("A")]
        [TestCase("ThisIsVeryLongFirstNa")]
        public void UpdateFirstName_ShouldThrowInvalidUserException_WhenFirstNameLengthIsInvalid(string firstName)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.UpdateFirstName(firstName));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(firstName)} must have between {MinNameLength} and {MaxNameLength} symbols."));
        }

        [TestCase("John")]
        [TestCase("Su")]
        [TestCase("VeryLongLongLongName")]
        public void UpdateLastName_ShouldUpdateLastName_WhenLastNameIsValid(string lastName)
        {
            _user.UpdateLastName(lastName);

            Assert.That(_user.LastName, Is.EqualTo(lastName));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateLastName_ShouldThrowInvalidUserException_WhenLastNameIsNullOrWhiteSpace(string lastName)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.UpdateLastName(lastName));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(lastName)} cannot be null or empty."));
        }

        [TestCase("A")]
        [TestCase("ThisIsVeryLongLastName")]
        public void UpdateLastName_ShouldThrowInvalidUserException_WhenLastNameLengthIsInvalid(string lastName)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.UpdateLastName(lastName));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(lastName)} must have between {MinNameLength} and {MaxNameLength} symbols."));
        }

        [Test]
        public void UpdatePasswordHash_ShouldUpdatePasswordHash_WhenPasswordHashIsValid()
        {
            var hashedPassword = "AQAAAAEAACcQAAAAEAnotherDummyHashValue==";

            _user.UpdatePasswordHash(hashedPassword);

            Assert.That(_user.PasswordHash, Is.EqualTo(hashedPassword));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdatePasswordHash_ShouldThrowInvalidUserException_WhenPasswordHashIsNullOrWhiteSpace(string passwordHash)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.UpdatePasswordHash(passwordHash));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(passwordHash)} cannot be null or empty."));
        }

        [TestCase("test@gmail.com", "0884787878")]
        [TestCase("mail@mail.bg", "+359884787878")]
        public void UpdateContactInfo_ShouldUpdateContactInfo_WhenEmailAndPhoneNumberAreValid(string email, string phoneNumber)
        {
            _user.UpdateContactInfo(email, phoneNumber);

            Assert.That(_user.ContactInfo.Email, Is.EqualTo(email));
            Assert.That(_user.ContactInfo.PhoneNumber, Is.EqualTo(phoneNumber));
        }

        [Test]
        public void UpdateLastLogin_ShouldUpdateLastLoginAt_WhenCalled()
        {
            Assert.That(_user.LastLoginAt, Is.Null);

            _user.UpdateLastLogin(Now);

            Assert.That(_user.LastLoginAt, Is.EqualTo(Now));

            var later = Now.AddMinutes(5);
            _user.UpdateLastLogin(later);

            Assert.That(_user.LastLoginAt, Is.EqualTo(later));
        }

        [Test]
        public void UpdateLastLogin_ShouldThrowArgumentException_WhenTimeIsNotUtc()
        {
            var localNow = DateTime.SpecifyKind(Now, DateTimeKind.Local);

            Assert.Throws<ArgumentException>(() => _user.UpdateLastLogin(localNow));
        }

        [Test]
        public void ConfirmEmail_ShouldConfirmEmail_WhenTokenIsValid()
        {
            Assert.That(_user.IsEmailConfirmed, Is.False);

            _user.ConfirmEmail(_user.EmailConfirmationToken, Now);

            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [Test]
        public void ConfirmEmail_ShouldConfirmEmail_JustBeforeTokenExpires()
        {
            var justBeforeExpiry = _user.EmailConfirmationTokenExpiry.Value.AddTicks(-1);

            _user.ConfirmEmail(_user.EmailConfirmationToken, justBeforeExpiry);

            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(60 * 60)]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenTokenHasExpired(int secondsAfterExpiry)
        {
            var now = _user.EmailConfirmationTokenExpiry.Value.AddSeconds(secondsAfterExpiry);

            var ex = Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(_user.EmailConfirmationToken, now));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
            Assert.That(_user.IsEmailConfirmed, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("wrong-token")]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenTokenIsInvalid(string token)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(token, Now));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
            Assert.That(_user.IsEmailConfirmed, Is.False);
        }

        [Test]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenTokenDiffersOnlyByCase()
        {
            var token = _user.EmailConfirmationToken.ToUpperInvariant();

            Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(token, Now));
        }

        [Test]
        public void ConfirmEmail_ShouldNotThrow_WhenAlreadyConfirmedWithSameToken()
        {
            var token = _user.EmailConfirmationToken;
            _user.ConfirmEmail(token, Now);

            Assert.DoesNotThrow(() => _user.ConfirmEmail(token, Now));
            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [Test]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenAlreadyConfirmedWithDifferentToken()
        {
            _user.ConfirmEmail(_user.EmailConfirmationToken, Now);

            var ex = Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail("any-token", Now));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldReplaceTokenAndExtendExpiry_WhenEmailIsNotConfirmed()
        {
            var oldToken = _user.EmailConfirmationToken;

            var later = Now.AddHours(30);
            _user.RegenerateEmailConfirmationToken(later);

            Assert.That(_user.EmailConfirmationToken, Is.Not.EqualTo(oldToken));
            Assert.That(Guid.TryParse(_user.EmailConfirmationToken, out _), Is.True);
            Assert.That(_user.EmailConfirmationTokenExpiry, Is.EqualTo(later.AddHours(EmailConfirmationTokenLifetimeHours)));
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldAllowConfirmation_AfterOldTokenExpired()
        {
            var afterExpiry = Now.AddHours(EmailConfirmationTokenLifetimeHours + 1);

            _user.RegenerateEmailConfirmationToken(afterExpiry);

            Assert.DoesNotThrow(() => _user.ConfirmEmail(_user.EmailConfirmationToken, afterExpiry));
            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldInvalidateOldToken()
        {
            var oldToken = _user.EmailConfirmationToken;

            _user.RegenerateEmailConfirmationToken(Now);

            Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(oldToken, Now));
            Assert.DoesNotThrow(() => _user.ConfirmEmail(_user.EmailConfirmationToken, Now));
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldDoNothing_WhenEmailIsAlreadyConfirmed()
        {
            var token = _user.EmailConfirmationToken;
            var expiry = _user.EmailConfirmationTokenExpiry;
            _user.ConfirmEmail(token, Now);

            _user.RegenerateEmailConfirmationToken(Now);

            Assert.That(_user.EmailConfirmationToken, Is.EqualTo(token));
            Assert.That(_user.EmailConfirmationTokenExpiry, Is.EqualTo(expiry));
            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [Test]
        public void AssignTrainerRole_ShouldAddTrainerRole_WhenUserIsNotTrainer()
        {
            var trainerRole = Role.Create(TrainerRoleName);

            _user.AssignTrainerRole(trainerRole);

            Assert.That(_user.HasRole(trainerRole.Id), Is.True);
        }

        [Test]
        public void AssignTrainerRole_ShouldThrowInvalidUserException_WhenUserIsAlreadyTrainer()
        {
            var trainerRole = Role.Create(TrainerRoleName);
            _user.AssignTrainerRole(trainerRole);

            var ex = Assert.Throws<InvalidUserException>(() => _user.AssignTrainerRole(trainerRole));

            Assert.That(ex.Error, Is.EqualTo(UserAlreadyTrainer));
            Assert.That(_user.Roles, Has.Count.EqualTo(1));
        }

        [Test]
        public void AssignTrainerRole_ShouldThrowInvalidUserException_WhenRoleIsNull()
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.AssignTrainerRole(null));

            Assert.That(ex.Error, Is.EqualTo("trainerRole cannot be the default value."));
        }

        [Test]
        public void AssignTrainerRole_ShouldThrowInvalidUserException_WhenRoleIsNotTrainer()
        {
            var nonTrainerRole = Role.Create("Administrator");

            var ex = Assert.Throws<InvalidUserException>(() => _user.AssignTrainerRole(nonTrainerRole));

            Assert.That(ex.Error, Is.EqualTo(RoleIsNotTrainer));
            Assert.That(_user.Roles, Is.Empty);
        }

        [Test]
        public void RevokeTrainerRole_ShouldRemoveTrainerRole_WhenUserIsTrainer()
        {
            var trainerRole = Role.Create(TrainerRoleName);
            _user.AssignTrainerRole(trainerRole);

            _user.RevokeTrainerRole(trainerRole);

            Assert.That(_user.HasRole(trainerRole.Id), Is.False);
            Assert.That(_user.Roles, Is.Empty);
        }

        [Test]
        public void RevokeTrainerRole_ShouldThrowInvalidUserException_WhenUserIsNotInTrainerRole()
        {
            var trainerRole = Role.Create(TrainerRoleName);

            var ex = Assert.Throws<InvalidUserException>(() => _user.RevokeTrainerRole(trainerRole));

            Assert.That(ex.Error, Is.EqualTo(UserNotInRole));
        }

        [Test]
        public void RevokeTrainerRole_ShouldThrowInvalidUserException_WhenRoleIsNotTrainer()
        {
            var nonTrainerRole = Role.Create("Administrator");
            _user.AddRole(nonTrainerRole);

            var ex = Assert.Throws<InvalidUserException>(() => _user.RevokeTrainerRole(nonTrainerRole));

            Assert.That(ex.Error, Is.EqualTo(RoleIsNotTrainer));
            Assert.That(_user.HasRole(nonTrainerRole.Id), Is.True);
        }

        [Test]
        public void Create_ShouldRaiseUserRegisteredDomainEvent()
        {
            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<UserRegisteredDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldRaiseDomainEvent_WhenEmailIsNotConfirmed()
        {
            _user.ClearDomainEvents();

            _user.RegenerateEmailConfirmationToken(Now);

            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<EmailConfirmationTokenRegeneratedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
        }

        [Test]
        public void RegenerateEmailConfirmationToken_ShouldNotRaiseDomainEvent_WhenEmailIsAlreadyConfirmed()
        {
            _user.ConfirmEmail(_user.EmailConfirmationToken, Now);
            _user.ClearDomainEvents();

            _user.RegenerateEmailConfirmationToken(Now);

            Assert.That(_user.DomainEvents, Is.Empty);
        }

        [Test]
        public void AssignTrainerRole_ShouldRaiseDomainEvent()
        {
            _user.ClearDomainEvents();

            _user.AssignTrainerRole(Role.Create(TrainerRoleName));

            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<TrainerRoleAssignedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
        }

        [Test]
        public void RevokeTrainerRole_ShouldRaiseDomainEvent()
        {
            var trainerRole = Role.Create(TrainerRoleName);
            _user.AssignTrainerRole(trainerRole);
            _user.ClearDomainEvents();

            _user.RevokeTrainerRole(trainerRole);

            Assert.That(_user.DomainEvents, Has.Count.EqualTo(1));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<TrainerRoleRevokedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
        }

        [Test]
        public void RevokeTrainerRole_ShouldNotRaiseDomainEvent_WhenUserIsNotInTrainerRole()
        {
            _user.ClearDomainEvents();

            Assert.Throws<InvalidUserException>(() => _user.RevokeTrainerRole(Role.Create(TrainerRoleName)));

            Assert.That(_user.DomainEvents, Is.Empty);
        }

        private string RequestReset(DateTime? at = null)
        {
            _user.ClearDomainEvents();
            _user.RequestPasswordReset(at ?? Now);
            return ((PasswordResetRequestedDomainEvent)_user.DomainEvents[0]).Token;
        }

        [Test]
        public void RequestPasswordReset_ShouldStoreOnlyTheTokenHashAndRaiseEventWithRawToken()
        {
            var token = RequestReset();

            Assert.That(token, Has.Length.GreaterThanOrEqualTo(43));
            Assert.That(_user.PasswordResetTokenHash, Has.Length.EqualTo(PasswordResetTokenHashLength));
            Assert.That(_user.PasswordResetTokenHash, Does.Not.Contain(token));
            Assert.That(_user.PasswordResetTokenExpiry, Is.EqualTo(Now.AddHours(PasswordResetTokenLifetimeHours)));
            Assert.That(_user.DomainEvents[0], Is.TypeOf<PasswordResetRequestedDomainEvent>().And.Property("UserId").EqualTo(_user.Id));
        }

        [Test]
        public void RequestPasswordReset_ShouldGenerateADifferentTokenEachTime()
        {
            Assert.That(RequestReset(), Is.Not.EqualTo(RequestReset()));
        }

        [Test]
        public void ResetPassword_ShouldReplacePasswordHashAndClearToken_WhenTokenIsValid()
        {
            var token = RequestReset();

            _user.ResetPassword(token, "AQAAAAEAACcQAAAAENewHashValue==", Now.AddMinutes(30));

            Assert.That(_user.PasswordHash, Is.EqualTo("AQAAAAEAACcQAAAAENewHashValue=="));
            Assert.That(_user.PasswordResetTokenHash, Is.Null);
            Assert.That(_user.PasswordResetTokenExpiry, Is.Null);
        }

        [Test]
        public void ResetPassword_ShouldThrow_WhenTokenIsReused()
        {
            var token = RequestReset();
            _user.ResetPassword(token, "AQAAAAEAACcQAAAAENewHashValue==", Now);

            var ex = Assert.Throws<InvalidUserException>(() => _user.ResetPassword(token, "AQAAAAEAACcQAAAAEOtherHashValue==", Now));
            Assert.That(ex.Error, Is.EqualTo(PasswordResetTokenInvalid));
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ResetPassword_ShouldThrowAndKeepPassword_WhenTokenHasExpired(int secondsAfterExpiry)
        {
            var token = RequestReset();
            var at = Now.AddHours(PasswordResetTokenLifetimeHours).AddSeconds(secondsAfterExpiry);

            Assert.Throws<InvalidUserException>(() => _user.ResetPassword(token, "AQAAAAEAACcQAAAAENewHashValue==", at));
            Assert.That(_user.PasswordHash, Is.EqualTo(ValidPasswordHash));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not-the-token")]
        public void ResetPassword_ShouldThrow_WhenTokenIsWrong(string token)
        {
            RequestReset();

            Assert.Throws<InvalidUserException>(() => _user.ResetPassword(token, "AQAAAAEAACcQAAAAENewHashValue==", Now));
            Assert.That(_user.PasswordHash, Is.EqualTo(ValidPasswordHash));
        }

        [Test]
        public void ResetPassword_ShouldThrow_WhenNoResetWasRequested()
        {
            Assert.Throws<InvalidUserException>(() => _user.ResetPassword("anything", "AQAAAAEAACcQAAAAENewHashValue==", Now));
        }

        [Test]
        public void ResetPassword_ShouldRejectTheOldToken_AfterANewRequest()
        {
            var oldToken = RequestReset();
            var newToken = RequestReset();

            Assert.Throws<InvalidUserException>(() => _user.ResetPassword(oldToken, "AQAAAAEAACcQAAAAENewHashValue==", Now));
            Assert.DoesNotThrow(() => _user.ResetPassword(newToken, "AQAAAAEAACcQAAAAENewHashValue==", Now));
        }
    }
}
