using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IForumReadRepository
    {
        Task<PagedResult<ForumTopicDto>> GetAllAsync(int page, int pageSize, bool includeDeleted, CancellationToken cancellationToken);
    }
}
