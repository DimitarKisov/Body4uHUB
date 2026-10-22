using Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories;

internal sealed class BookmarkReadRepository(ContentDbContext dbContext) : IBookmarkReadRepository
{
    private readonly ContentDbContext _dbContext = dbContext;

    public async Task<PagedResult<GetUserBookmarksResponse>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Bookmarks of articles that are no longer published stay in the database but are not shown,
        // so they reappear if an archived article is published again.
        var query = from b in _dbContext.Bookmarks
                    join a in _dbContext.Articles on b.ArticleId equals a.Id
                    where b.UserId == userId && a.Status == ArticleStatus.Published && !a.IsDeleted
                    select new { Bookmark = b, Article = a };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Bookmark.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new GetUserBookmarksResponse(
                x.Bookmark.Id,
                x.Article.Id,
                x.Article.Title,
                x.Article.AuthorId,
                x.Bookmark.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<GetUserBookmarksResponse>(items, totalCount, page, pageSize);
    }
}
