using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.ValueObjects;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Domain.UnitTests
{
    [TestFixture]
    public class UserUnitTests
    {
        private User _user;

        [SetUp]
        public void Setup()
        {
            _user = User.Create(
                "samoRandomNotHashedPassword",
                "Test",
                "User",
                "test@mail.com",
                "0884787878",
                "testToken");
        }

        [TestCase("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", "test@mail.com", "0884787878", "someRandomConfirmationToken")]
        [TestCase("AQAAAAEAACcQAAAAEDummyHashValue==", "Test", "User", "test@mail.com", "+359884787878", "someRandomConfirmationToken")]
        public void Create_ShouldCreateUser_WhenAllParametersAreValid(string passwordHash, string firstName, string lastName, string email, string phoneNumber, string confirmationToken)
        {
            var result = User.Create(
                passwordHash,
                firstName,
                lastName,
                email,
                phoneNumber,
                confirmationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PasswordHash, Is.EqualTo(passwordHash));
            Assert.That(result.FirstName, Is.EqualTo(firstName));
            Assert.That(result.LastName, Is.EqualTo(lastName));
            Assert.That(result.ContactInfo.Email, Is.EqualTo(email));
            Assert.That(result.ContactInfo.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(result.EmailConfirmationToken, Is.EqualTo(confirmationToken));
        }

        [Test]
        public void Create_ShouldThrowInvalidUserExceptionAndMessageAgainstEmptyString_WhenPasswordHashIsNullOrWhiteSpace()
        {
            var result = Assert.Throws<InvalidUserException>(() => 
                User.Create(
                    null,
                    "Test",
                    "User",
                    "test@mail.com",
                    "0884787878",
                    "testToken")
                );

            Assert.That(result.Error, Is.EqualTo($"passwordHash cannot be null or empty."));
        }

        [Test]
        public void Create_ShouldThrowInvalidUserExceptionAndMessageAgainstEmptyString_WhenFirstNameIsNullOrWhiteSpace()
        {
            var result = Assert.Throws<InvalidUserException>(() =>
                User.Create(
                    "AQAAAAEAACcQAAAAEDummyHashValue==",
                    null,
                    "User",
                    "test@mail.com",
                    "0884787878",
                    "testToken")
                );

            Assert.That(result.Error, Is.EqualTo($"firstName cannot be null or empty."));
        }

        [TestCase("AQAAAAEAACcQAAAAEDummyHashValue==", "A", "User", "test@mail.com", "0884787878", "someRandomConfirmationToken")]
        [TestCase("AQAAAAEAACcQAAAAEDummyHashValue==", "ThisIsVeryLongFirstNa", "User", "test@mail.com", "0884787878", "someRandomConfirmationToken")]
        public void Create_ShouldThrowInvalidUserExceptionAndMessageForStringLength_WhenFirstNameIsInvalid(string passwordHash, string firstName, string lastName, string email, string phoneNumber, string confirmationToken)
        {
            var result = Assert.Throws<InvalidUserException>(() =>
               User.Create(
                   passwordHash,
                   firstName,
                   lastName,
                   email,
                   phoneNumber,
                   confirmationToken)
               );

            Assert.That(result.Error, Is.EqualTo($"firstName must have between {MinNameLength} and {MaxNameLength} symbols."));
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
        public void UpdateFirstName_ShouldThrowInvalidUserExceptionAndMessageAgainstEmptyString_WhenFirstNameIsNullOrWhiteSpace(string firstName)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdateFirstName(firstName));

            Assert.That(result.Error, Is.EqualTo($"{nameof(firstName)} cannot be null or empty."));
        }

        [TestCase("A")]
        [TestCase("ThisIsVeryLongFirstNa")]
        public void UpdateFirstName_ShouldThrowInvalidUserExceptionAndMessageAgainstStringLength_FirstNameIsInvalid(string firstName)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdateFirstName(firstName));

            Assert.That(result.Error, Is.EqualTo($"{nameof(firstName)} must have between {MinNameLength} and {MaxNameLength} symbols."));
        }

        [TestCase("John")]
        [TestCase("Su")]
        [TestCase("VeryLongLongLongName")]
        public void UpdateLastName_ShouldUpdateFirstName_WhenLastNameIsValid(string lastName)
        {
            _user.UpdateLastName(lastName);

            Assert.That(_user.LastName, Is.EqualTo(lastName));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdateLastName_ShouldThrowInvalidUserExceptionAndMessageAgainstEmptyString_WhenLastNameIsNullOrWhiteSpace(string lastName)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdateLastName(lastName));

            Assert.That(result.Error, Is.EqualTo($"{nameof(lastName)} cannot be null or empty."));
        }

        [TestCase("A")]
        [TestCase("ThisIsVeryLongLastName")]
        public void UpdateLastName_ShouldThrowInvalidUserExceptionAndMessageAgainstStringLength_LastNameIsInvalid(string lastName)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdateLastName(lastName));

            Assert.That(result.Error, Is.EqualTo($"{nameof(lastName)} must have between {MinNameLength} and {MaxNameLength} symbols."));
        }

        [Test]
        public void UpdatePasswordHash_ShouldUpdatePasswordHash_WhenPasswordHashIsValid()
        {
            var hasshedPassword = "AQAAAAEAACcQAAAAEDummyHashValue==";

            _user.UpdatePasswordHash(hasshedPassword);

            Assert.That(_user.PasswordHash, Is.EqualTo(hasshedPassword));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void UpdatePasswordHash_ShouldThrowInvalidUserExceptionAndMessageAgainstEmptyString_WhenPasswordHashIsNullOrWhiteSpace(string passwordHash)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdatePasswordHash(passwordHash));

            Assert.That(result.Error, Is.EqualTo($"{nameof(passwordHash)} cannot be null or empty."));
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
            
            var begfore2 = DateTime.UtcNow;
            _user.UpdateLastLogin();
            var after2 = DateTime.UtcNow;

            Assert.That(_user.LastLoginAt, Is.InRange(begfore2, after2));
            Assert.That(_user.LastLoginAt, Is.GreaterThan(firstLastLogin));
        }

        [Test]

        public void ConfirmEmail_ShouldSetIsEmailConfirmedToTrue_WhenCalled()
        {
            Assert.That(_user.IsEmailConfirmed, Is.False);

            _user.ConfirmEmail();

            Assert.That(_user.IsEmailConfirmed, Is.True);
        }
    }
}
