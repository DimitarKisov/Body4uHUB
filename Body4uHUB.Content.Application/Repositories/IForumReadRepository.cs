using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IForumReadRepository
    {
        Task<IEnumerable<ForumTopicDto>> GetAllAsync(int skip, int take, CancellationToken cancellationToken);
    }
}
