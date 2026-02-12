using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;

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
        public void UpdateFirstName_ShouldThrowInvalidUserExceptionAndMessageAgainstStringLength_FirstNameIsValid(string firstName)
        {
            var result = Assert.Throws<InvalidUserException>(() => _user.UpdateFirstName(firstName));

            Assert.That(result.Error, Is.EqualTo($"{nameof(firstName)} must have between {MinNameLength} and {MaxNameLength} symbols."));
        }
    }
}
