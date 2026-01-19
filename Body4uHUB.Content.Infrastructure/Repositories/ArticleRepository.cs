using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Domain.ValueObjects;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class ArticleRepository : IArticleRepository
    {
        private readonly ContentDbContext _dbContext;

        public ArticleRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Article article)
        {
            _dbContext.Articles.Add(article);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNumberAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles.AnyAsync(x => x.ArticleNumber == id, cancellationToken);
        }

        public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles.AnyAsync(x => x.Title == title, cancellationToken);
        }

        public async Task<Guid> GetArticleIdByNumberAsync(int number, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Where(x => x.ArticleNumber == number)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Article> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Article> GetByNumberAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.ArticleNumber == id, cancellationToken);
        }

        public void Remove(Article article)
        {
            _dbContext.Articles.Remove(article);
        }
    }
}