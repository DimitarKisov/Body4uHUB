using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IForumRepository : IRepository<ForumTopic>
    {
        void Add(ForumTopic forumTopic);
        Task<ForumTopic> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ForumTopic> GetByIdWithPostsAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default);
        void Remove(ForumTopic forumTopic);
    }
}