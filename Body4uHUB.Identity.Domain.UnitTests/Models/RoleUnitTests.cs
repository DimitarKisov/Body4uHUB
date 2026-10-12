using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Models;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Domain.UnitTests.Models
{
    [TestFixture]
    public class RoleUnitTests
    {
        [TestCase(TrainerRoleName)]
        [TestCase("Administrator")]
        public void Create_ShouldCreateRole_WhenNameIsValid(string name)
        {
            var role = Role.Create(name);

            Assert.That(role.Name, Is.EqualTo(name));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Create_ShouldThrowInvalidRoleExceptionAgainstEmptyString_WhenNameIsEmpty(string name)
        {
            var ex = Assert.Throws<InvalidRoleException>(() => Role.Create(name));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(Role.Name)} cannot be null or empty."));
        }

        [TestCase(MinRoleNameLength - 1)]
        [TestCase(MaxRoleNameLength + 1)]
        public void Create_ShouldThrowInvalidRoleExceptionForStringLength_WhenNameLengthIsOutOfRange(int length)
        {
            var ex = Assert.Throws<InvalidRoleException>(() => Role.Create(new string('a', length)));

            Assert.That(ex.Error, Is.EqualTo($"{nameof(Role.Name)} must have between {MinRoleNameLength} and {MaxRoleNameLength} symbols."));
        }
    }
}
