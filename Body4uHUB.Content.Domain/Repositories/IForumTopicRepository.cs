using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Content.Domain.Repositories
{
    public interface IForumTopicRepository : IRepository<ForumTopic>
    {
        void Add(ForumTopic forumTopic);
        Task<ForumTopic> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ForumTopic>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<IEnumerable<ForumTopic>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        void Remove(ForumTopic forumTopic);
    }
}