using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class ArticleReadRepository : IArticleReadRepository
    {
        private readonly ContentDbContext _dbContext;

        public ArticleReadRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.Articles
                .Where(x => x.Status == ArticleStatus.Published)
                .OrderByDescending(x => x.PublishedAt)
                .Skip(skip)
                .Take(take)
                .Select(x => new ArticleDto
                {
                    Id = x.ArticleNumber,
                    Title = x.Title,
                    Content = x.Content,
                    AuthorId = x.AuthorId,
                    Status = x.Status.Name,
                    PublishedAt = x.PublishedAt,
                    ViewCount = x.ViewCount,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ArticleDto>> GetArticlesByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Where(x => x.AuthorId == authorId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(x => new ArticleDto
                {
                    Id = x.ArticleNumber,
                    Title = x.Title,
                    Content = x.Content,
                    AuthorId = x.AuthorId,
                    Status = x.Status.Name,
                    PublishedAt = x.PublishedAt,
                    ViewCount = x.ViewCount,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
