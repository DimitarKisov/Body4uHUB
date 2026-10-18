using Body4uHUB.Identity.Application.Commands.ResetPassword;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ResetPasswordCommandValidatorTests
    {
        private readonly ResetPasswordCommandValidator _validator = new();

        [Test]
        public void Validate_ShouldPass_WhenAllFieldsAreValid()
        {
            Assert.That(_validator.Validate(new ResetPasswordCommand("test@mail.com", "token", "NewPassword1!")).IsValid, Is.True);
        }

        [TestCase("", "token", "NewPassword1!", nameof(ResetPasswordCommand.Email))]
        [TestCase("test@mail.com", "", "NewPassword1!", nameof(ResetPasswordCommand.Token))]
        [TestCase("test@mail.com", "token", "weak", nameof(ResetPasswordCommand.NewPassword))]
        public void Validate_ShouldFailOnTheField(string email, string token, string newPassword, string field)
        {
            var result = _validator.Validate(new ResetPasswordCommand(email, token, newPassword));

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Select(e => e.PropertyName).Distinct(), Is.EqualTo(new[] { field }));
        }
    }
}
