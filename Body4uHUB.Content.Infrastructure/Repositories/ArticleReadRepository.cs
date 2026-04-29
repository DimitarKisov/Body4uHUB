using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Models;
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

        public async Task<IEnumerable<GetArticlesByAuthorResponse>> GetArticlesByAuthorAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Where(x => x.AuthorId == authorId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(x => new GetArticlesByAuthorResponse
                (
                    x.ArticleNumber,
                    x.Title,
                    x.AuthorId,
                    x.Status.Name,
                    x.PublishedAt,
                    x.ViewCount,
                    x.CreatedAt,
                    x.ModifiedAt)
                )
                .ToListAsync(cancellationToken);
        }

        public async Task<ArticleDto> GetByNumberAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Select(x=> new ArticleDto
                {
                    Id = x.ArticleNumber,
                    Title = x.Title,
                    Content = x.Content,
                    AuthorId = x.AuthorId,
                    Status = x.Status.Name,
                    PublishedAt = x.PublishedAt,
                    ViewCount = x.ViewCount + 1,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt,
                    Comments = x.Comments.Select(y => new CommentDto
                    {
                        Id = y.Id,
                        AuthorId = y.AuthorId,
                        Content = y.Content,
                        CreatedAt = y.CreatedAt,
                        ModifiedAt = y.ModifiedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
