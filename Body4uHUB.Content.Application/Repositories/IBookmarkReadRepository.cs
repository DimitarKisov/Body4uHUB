using Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IBookmarkReadRepository
    {
        Task<PagedResult<GetUserBookmarksResponse>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
