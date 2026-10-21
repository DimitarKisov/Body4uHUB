using Body4uHUB.Content.Application.Abstractions;

namespace Body4uHUB.Content.Infrastructure.Persistence
{
    /// <summary>
    /// Marks that a visitor has already been counted for a target on a given day (Europe/Sofia).
    /// Only today's rows are needed; older ones are removed by ContentViewCleanupService.
    /// </summary>
    internal sealed class ContentView
    {
        public ViewTarget TargetType { get; init; }
        public int TargetId { get; init; }
        public byte[] VisitorHash { get; init; }
        public DateOnly ViewDate { get; init; }
    }
}
