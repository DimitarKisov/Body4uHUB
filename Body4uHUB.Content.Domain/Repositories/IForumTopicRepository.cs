using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IForumTopicRepository : IRepository<ForumTopic>
    {
        void Add(ForumTopic forumTopic);
        Task<ForumTopic> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ForumTopic> GetByIdWithPostsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        void Remove(ForumTopic forumTopic);
    }
}