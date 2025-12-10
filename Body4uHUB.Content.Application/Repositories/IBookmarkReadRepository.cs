using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Repositories
{
    public interface IBookmarkReadRepository
    {
        Task<IEnumerable<BookmarkDto>> GetByUserIdAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default);
    }
}
