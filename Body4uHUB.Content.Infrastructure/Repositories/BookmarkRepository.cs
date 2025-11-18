namespace Body4uHUB.Content.Infrastructure.Repositories
{
    using Body4uHUB.Content.Domain.Models;
    using Body4uHUB.Content.Domain.Repositories;
    using Body4uHUB.Content.Domain.ValueObjects;
    using Body4uHUB.Content.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    internal class BookmarkRepository : IBookmarkRepository
    {
        private readonly ContentDbContext _dbContext;

        public BookmarkRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Bookmark bookmark)
        {
            _dbContext.Bookmarks.Add(bookmark);
        }

        public async Task<bool> ExistsAsync(Guid userId, ArticleId articleId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Bookmarks.AnyAsync(x => x.UserId == userId && x.ArticleId == articleId, cancellationToken);
        }

        public async Task<Bookmark> GetByUserAndArticleAsync(Guid userId, ArticleId articleId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Bookmarks.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId, cancellationToken);
        }

        public void Remove(Bookmark bookmark)
        {
            _dbContext.Bookmarks.Remove(bookmark);
        }
    }
}