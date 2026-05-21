using Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class BookmarkReadRepository : IBookmarkReadRepository
    {
        private readonly ContentDbContext _dbContext;

        public BookmarkReadRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<GetUserBookmarksResponse>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Bookmarks.Where(b => b.UserId == userId);

            var totalCount = query.Count();

            var items = await (from b in query
                               join a in _dbContext.Articles on b.ArticleId equals a.Id
                               orderby b.CreatedAt descending
                               select new GetUserBookmarksResponse(
                                   b.Id,
                                   a.Id,
                                   a.Title,
                                   a.AuthorId,
                                   b.CreatedAt))
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync(cancellationToken);

            return new PagedResult<GetUserBookmarksResponse>(items, totalCount, page, pageSize);
        }
    }
}
