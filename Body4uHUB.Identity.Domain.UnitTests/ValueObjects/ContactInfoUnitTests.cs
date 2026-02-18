using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.ValueObjects;

namespace Body4uHUB.Identity.Domain.UnitTests.ValueObjects
{
    [TestFixture]
    public class ContactInfoUnitTests
    {
        private const string ValidEmail = "test@mail.com";
        private const string ValidEmail2 = "mail@abv.bg";
        private const string ValidPhoneNumber = "0884787878";
        private const string ValidPhoneNumber2 = "+359884787878";

        [TestCase(ValidEmail, ValidPhoneNumber)]
        [TestCase(ValidEmail2, ValidPhoneNumber2)]
        public void Create_ShouldCreateContactInfo_WhenAllParametersAreValid(string email, string phoneNumber)
        {
            var resultWithPhoneNumber = ContactInfo.Create(email, phoneNumber);

            Assert.That(resultWithPhoneNumber, Is.Not.Null);
            Assert.That(resultWithPhoneNumber.Email, Is.EqualTo(email));
            Assert.That(resultWithPhoneNumber.PhoneNumber, Is.EqualTo(phoneNumber));

            var resultWithNullPhoneNumber = ContactInfo.Create(email, null);

            Assert.That(resultWithNullPhoneNumber, Is.Not.Null);
            Assert.That(resultWithNullPhoneNumber.Email, Is.EqualTo(email));
            Assert.That(resultWithNullPhoneNumber.PhoneNumber, Is.Null);
        }

        [TestCase("", ValidPhoneNumber)]
        [TestCase(" ", ValidPhoneNumber)]
        [TestCase(null, ValidPhoneNumber)]
        public void Create_ShouldThrowInvalidContactInfoExceptionAgainstEmptyString_WhenEmailIsInvalid(string email, string phoneNumber)
        {
            var ex = Assert.Throws<InvalidContactInfoException>(() => ContactInfo.Create(email, phoneNumber));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(email)} cannot be null or empty."));
        }

        [TestCase("mail.com", ValidPhoneNumber)]
        [TestCase("test@", ValidPhoneNumber)]
        [TestCase("test@mail", ValidPhoneNumber)]
        [TestCase("te$tmail.com", ValidPhoneNumber)]
        public void Create_ShouldThrowInvalidContactInfoExceptionAgainstNotContainingSpecialChars_WhenEmailIsInvalid(string email, string phoneNumber)
        {
            var ex = Assert.Throws<InvalidContactInfoException>(() => ContactInfo.Create(email, ValidPhoneNumber));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(email)} is not in a valid email format."));
        }

        [TestCase(ValidEmail, "088748484")]
        [TestCase(ValidEmail, "08874848488")]
        [TestCase(ValidEmail, "12345678901234567890")]
        [TestCase(ValidEmail, "phone123")]
        [TestCase(ValidEmail, "-359894484848")]
        [TestCase(ValidEmail, "+3598874848")]
        public void Create_ShouldThrowInvalidContactInfoExceptionAgainstNotMatchingRegex_WhenPhoneNumberIsInvalid(string email, string phoneNumber)
        {
            var ex = Assert.Throws<InvalidContactInfoException>(() => ContactInfo.Create(ValidEmail, phoneNumber));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(phoneNumber)} is invalid."));
        }
    }
}
