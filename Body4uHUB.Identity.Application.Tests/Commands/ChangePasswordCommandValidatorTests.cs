using Body4uHUB.Identity.Application.Commands.ChangePassword;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Commands
{
    [TestFixture]
    public class ChangePasswordCommandValidatorTests
    {
        private readonly ChangePasswordCommandValidator _validator = new();

        [Test]
        public void Validate_ShouldFailOnNewPassword_WhenItEqualsCurrentPassword()
        {
            var result = _validator.Validate(new ChangePasswordCommand(Guid.NewGuid(), "Password1!", "Password1!"));

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword) && e.ErrorMessage == NewPasswordSameAsCurrent), Is.True);
        }

        [Test]
        public void Validate_ShouldPass_WhenNewPasswordIsValidAndDifferent()
        {
            var result = _validator.Validate(new ChangePasswordCommand(Guid.NewGuid(), "Password1!", "NewPassword1!"));

            Assert.That(result.IsValid, Is.True);
        }
    }
}
