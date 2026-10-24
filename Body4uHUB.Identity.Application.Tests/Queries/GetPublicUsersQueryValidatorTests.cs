using Body4uHUB.Identity.Application.Queries.GetPublicUsers;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Tests.Queries
{
    [TestFixture]
    public class GetPublicUsersQueryValidatorTests
    {
        private readonly GetPublicUsersQueryValidator _validator = new();

        [Test]
        public void Validate_ShouldPass_WhenThereAreAtMostTheMaximumDistinctIds()
        {
            Assert.That(_validator.Validate(Query(MaxPublicUserIds)).IsValid, Is.True);
        }

        [Test]
        public void Validate_ShouldPass_WhenRepeatedIdsExceedTheMaximum()
        {
            var id = Guid.NewGuid();

            Assert.That(_validator.Validate(new GetPublicUsersQuery([.. Enumerable.Repeat(id, MaxPublicUserIds + 1)])).IsValid, Is.True);
        }

        [Test]
        public void Validate_ShouldFail_WhenThereAreMoreThanTheMaximumDistinctIds()
        {
            var result = _validator.Validate(Query(MaxPublicUserIds + 1));

            Assert.That(result.Errors.Select(e => e.ErrorMessage), Is.EqualTo(new[] { string.Format(PublicUserIdsLimit, MaxPublicUserIds) }));
        }

        [Test]
        public void Validate_ShouldFail_WhenThereAreNoIds()
        {
            Assert.That(_validator.Validate(new GetPublicUsersQuery([])).Errors.Select(e => e.ErrorMessage), Is.EqualTo(new[] { PublicUserIdsRequired }));
            Assert.That(_validator.Validate(new GetPublicUsersQuery(null)).Errors.Select(e => e.ErrorMessage), Is.EqualTo(new[] { PublicUserIdsRequired }));
        }

        [Test]
        public void Validate_ShouldFail_WhenAnIdIsEmpty()
        {
            var result = _validator.Validate(new GetPublicUsersQuery([Guid.NewGuid(), Guid.Empty]));

            Assert.That(result.Errors.Select(e => e.ErrorMessage), Is.EqualTo(new[] { UserIdRequired }));
        }

        private static GetPublicUsersQuery Query(int distinctIds)
        {
            return new GetPublicUsersQuery([.. Enumerable.Range(0, distinctIds).Select(_ => Guid.NewGuid())]);
        }
    }
}
