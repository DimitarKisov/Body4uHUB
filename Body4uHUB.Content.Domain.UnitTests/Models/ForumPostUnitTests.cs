using Body4uHUB.Content.Domain.Models;

namespace Body4uHUB.Content.Domain.UnitTests.Models;

[TestFixture]
public class ForumPostUnitTests
{
    [Test]
    public void Create_ShouldSetCreatedAt()
    {
        var post = ForumPost.Create("Valid post content", Guid.NewGuid());

        Assert.That(post.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
    }
}
