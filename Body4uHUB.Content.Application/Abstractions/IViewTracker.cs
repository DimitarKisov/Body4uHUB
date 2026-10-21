namespace Body4uHUB.Content.Application.Abstractions
{
    public enum ViewTarget : byte
    {
        Article = 1,
        ForumTopic = 2
    }

    public interface IViewTracker
    {
        /// <summary>
        /// Registers a view of the target by the visitor for the current day.
        /// Returns false when the same visitor has already been counted today.
        /// </summary>
        Task<bool> TryRegisterViewAsync(ViewTarget target, int targetId, string visitorIp, CancellationToken cancellationToken);
    }
}
