using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal sealed class ArticleRepository(ContentDbContext dbContext)
        : IArticleRepository
    {
        private readonly ContentDbContext _dbContext = dbContext;

        public void Add(Article article)
        {
            _dbContext.Articles.Add(article);
        }

        public async Task<bool> ExistsPublishedAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles.AnyAsync(x => x.Id == id && !x.IsDeleted && x.Status == ArticleStatus.Published, cancellationToken);
        }

        public async Task<Article> GetWithCommentsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Article> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default)
        {
            var articleId = await _dbContext.Articles
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.ViewCount, y => y.ViewCount + 1), cancellationToken);

            return articleId > 0;
        }
    }
}