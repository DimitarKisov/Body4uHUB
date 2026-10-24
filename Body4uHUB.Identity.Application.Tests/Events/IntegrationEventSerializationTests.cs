using Body4uHUB.Shared.Application.Events;
using System.Text.Json;

namespace Body4uHUB.Identity.Application.Tests.Events
{
    // MassTransit serializes messages with System.Text.Json, so this is what a consumer receives.
    [TestFixture]
    public class IntegrationEventSerializationTests
    {
        [Test]
        public void Deserialize_ShouldKeepThePublishersEventIdAndOccurredAt()
        {
            var published = new UserRegisteredEvent
            {
                UserId = Guid.NewGuid(),
                OccurredAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            };

            var received = JsonSerializer.Deserialize<UserRegisteredEvent>(JsonSerializer.Serialize(published));

            Assert.That(received.EventId, Is.EqualTo(published.EventId));
            Assert.That(received.OccurredAt, Is.EqualTo(published.OccurredAt));
            Assert.That(received.UserId, Is.EqualTo(published.UserId));
        }
    }
}
