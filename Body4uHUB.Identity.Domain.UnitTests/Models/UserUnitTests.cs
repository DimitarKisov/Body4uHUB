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

        [SetUp]
        public void Setup()
        {
            _user = User.Create(
                ValidPasswordHash,
                ValidFirstName,
                ValidLastName,
                ValidEmail,
                ValidPhone);
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
            var before = DateTime.UtcNow;

            var result = User.Create(
                passwordHash,
                firstName,
                lastName,
                email,
                phoneNumber);

            var after = DateTime.UtcNow;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PasswordHash, Is.EqualTo(passwordHash));
            Assert.That(result.FirstName, Is.EqualTo(firstName));
            Assert.That(result.LastName, Is.EqualTo(lastName));
            Assert.That(result.ContactInfo.Email, Is.EqualTo(email));
            Assert.That(result.ContactInfo.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(result.IsEmailConfirmed, Is.False);
            Assert.That(Guid.TryParse(result.EmailConfirmationToken, out _), Is.True);
            Assert.That(result.EmailConfirmationTokenExpiry, Is.Not.Null);
            Assert.That(
                result.EmailConfirmationTokenExpiry.Value,
                Is.InRange(before.AddHours(EmailConfirmationTokenLifetimeHours), after.AddHours(EmailConfirmationTokenLifetimeHours)));
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
                    ValidPhone));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(passwordHash)} cannot be null or empty."));
        }

        [Test]
        public void AddRole_ShouldAddRole_WhenRoleIsValid()
        {
            var role = Role.Create("TestRole");

            _user.AddRole(role);

            Assert.That(_user.Roles, Has.Member(role));
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
        public void RemoveRole_ShouldRemoveRole_WhenRoleExists()
        {
            var role = Role.Create("TestRole");

            _user.AddRole(role);
            _user.RemoveRole(role);

            Assert.That(_user.Roles, Has.No.Member(role));
        }

        [Test]
        public void RemoveRole_ShouldThrowInvalidaUserException_WhenRoleIsNull()
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.RemoveRole(null));

            Assert.That(ex.Error, Is.EqualTo($"role cannot be the default value."));
        }

        [Test]
        public void RemoveRole_ShouldDoNothing_WhenRoleDoesNotExist()
        {
            var existingRole = Role.Create("Existing");
            var missingRole = Role.Create("Missing");

            _user.AddRole(existingRole);

            _user.RemoveRole(missingRole);

            Assert.That(_user.Roles, Has.Member(existingRole));
            Assert.That(_user.Roles, Has.No.Member(missingRole));
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

            var before1 = DateTime.UtcNow;
            _user.UpdateLastLogin();
            var after1 = DateTime.UtcNow;

            Assert.That(_user.LastLoginAt, Is.Not.Null);
            Assert.That(_user.LastLoginAt, Is.InRange(before1, after1));

            var firstLastLogin = _user.LastLoginAt.Value;

            var before2 = DateTime.UtcNow;
            _user.UpdateLastLogin();
            var after2 = DateTime.UtcNow;

            Assert.That(_user.LastLoginAt, Is.InRange(before2, after2));
            Assert.That(_user.LastLoginAt, Is.GreaterThan(firstLastLogin));
        }

        [Test]
        public void ConfirmEmail_ShouldConfirmEmail_WhenTokenIsValid()
        {
            Assert.That(_user.IsEmailConfirmed, Is.False);

            _user.ConfirmEmail(_user.EmailConfirmationToken);

            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("wrong-token")]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenTokenIsInvalid(string token)
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(token));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
            Assert.That(_user.IsEmailConfirmed, Is.False);
        }

        [Test]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenTokenDiffersOnlyByCase()
        {
            var token = _user.EmailConfirmationToken.ToUpperInvariant();

            Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail(token));
        }

        [Test]
        public void ConfirmEmail_ShouldNotThrow_WhenAlreadyConfirmedWithSameToken()
        {
            var token = _user.EmailConfirmationToken;
            _user.ConfirmEmail(token);

            Assert.DoesNotThrow(() => _user.ConfirmEmail(token));
            Assert.That(_user.IsEmailConfirmed, Is.True);
        }

        [Test]
        public void ConfirmEmail_ShouldThrowInvalidUserException_WhenAlreadyConfirmedWithDifferentToken()
        {
            _user.ConfirmEmail(_user.EmailConfirmationToken);

            var ex = Assert.Throws<InvalidUserException>(() => _user.ConfirmEmail("any-token"));

            Assert.That(ex.Error, Is.EqualTo(EmailConfirmationTokenInvalid));
        }

        [Test]
        public void EnsureIsTrainer_ShouldNotThrow_WhenUserHasTrainerRole()
        {
            var trainerRole = Role.Create(TrainerRoleName);
            _user.AddRole(trainerRole);

            Assert.DoesNotThrow(() => _user.EnsureIsTrainer(trainerRole));
        }

        [Test]
        public void EnsureIsTrainer_ShouldThrowInvalidUserException_WhenRoleIsNull()
        {
            var ex = Assert.Throws<InvalidUserException>(() => _user.EnsureIsTrainer(null));

            Assert.That(ex.Error, Is.EqualTo("trainerRole cannot be the default value."));
        }

        [Test]
        public void EnsureIsTrainer_ShouldThrowInvalidUserException_WhenRoleIsNotTrainer()
        {
            var nonTrainerRole = Role.Create("Administrator");
            _user.AddRole(nonTrainerRole);

            var ex = Assert.Throws<InvalidUserException>(() => _user.EnsureIsTrainer(nonTrainerRole));

            Assert.That(ex.Error, Is.EqualTo(RoleIsNotTrainer));
        }

        [Test]
        public void EnsureIsTrainer_ShouldThrowInvalidUserException_WhenUserIsNotInTrainerRole()
        {
            var trainerRole = Role.Create(TrainerRoleName);

            var ex = Assert.Throws<InvalidUserException>(() => _user.EnsureIsTrainer(trainerRole));

            Assert.That(ex.Error, Is.EqualTo(UserNotInRole));
        }
    }
}
