using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories;

internal sealed class BookmarkRepository(ContentDbContext dbContext)
    : IBookmarkRepository
{
    private readonly ContentDbContext _dbContext = dbContext;

    public void Add(Bookmark bookmark)
    {
        _dbContext.Bookmarks.Add(bookmark);
    }

    public async Task<bool> ExistsAsync(Guid userId, int articleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookmarks.AnyAsync(x => x.UserId == userId && x.ArticleId == articleId, cancellationToken);
    }

    public async Task<Bookmark> GetByUserAndArticleAsync(Guid userId, int articleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId, cancellationToken);
    }

    public void Remove(Bookmark bookmark)
    {
        _dbContext.Bookmarks.Remove(bookmark);
    }
}
