using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Content.Domain.Repositories;

public interface IForumRepository : IRepository<ForumTopic>
{
    void Add(ForumTopic forumTopic);
    Task<ForumTopic> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ForumTopic> GetByIdWithPostAsync(int id, int postId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTitleAsync(string title, int? exceptTopicId = null, CancellationToken cancellationToken = default);
    Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default);
    void Remove(ForumTopic forumTopic);
}
