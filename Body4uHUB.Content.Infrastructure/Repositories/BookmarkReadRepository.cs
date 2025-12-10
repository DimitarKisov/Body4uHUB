using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
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

        public async Task<IEnumerable<BookmarkDto>> GetByUserIdAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
        {
            return await (from b in _dbContext.Bookmarks
                          join a in _dbContext.Articles on b.ArticleId equals a.Id
                          where b.UserId == userId
                          orderby b.CreatedAt descending
                          select new BookmarkDto
                          {
                              Id = b.Id,
                              UserId = b.UserId,
                              CreatedAt = b.CreatedAt,
                              Article = new ArticleDto
                              {
                                  Id = a.Id.Value,
                                  Title = a.Title,
                                  Content = a.Content,
                                  AuthorId = a.AuthorId,
                                  Status = a.Status.Name,
                                  PublishedAt = a.PublishedAt,
                                  ViewCount = a.ViewCount,
                                  CreatedAt = a.CreatedAt,
                                  ModifiedAt = a.ModifiedAt,
                                  Comments = null
                              }
                          })
                  .Skip(skip)
                  .Take(take)
                  .ToListAsync(cancellationToken);
        }
    }
}
