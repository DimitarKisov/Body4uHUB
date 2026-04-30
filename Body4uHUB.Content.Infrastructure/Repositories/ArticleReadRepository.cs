using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Articles.GetAll;
using Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor;
using Body4uHUB.Content.Application.Queries.Articles.GetById;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Domain.Enumerations;
using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
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

        public async Task<PagedResult<GetAllArticlesResponse>> GetAllArticlesAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dbContext.Articles.Where(x => !x.IsDeleted && x.Status == ArticleStatus.Published);

            var totalCount = query.Count();

            var items = await _dbContext.Articles
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllArticlesResponse(
                    x.ArticleNumber,
                    x.Title,
                    x.AuthorId,
                    x.Status.Name,
                    x.PublishedAt,
                    x.ViewCount,
                    x.CreatedAt,
                    x.ModifiedAt
                    ))
                .ToListAsync(cancellationToken);

            return new PagedResult<GetAllArticlesResponse>(items, totalCount, page, pageSize);
        }

        public async Task<PagedResult<GetArticlesByAuthorResponse>> GetArticlesByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Articles
                .Where(x => x.AuthorId == authorId &&
                            x.Status == ArticleStatus.Published &&
                            !x.IsDeleted);

            var totalCount = query.Count();

            var items = await _dbContext.Articles
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetArticlesByAuthorResponse(
                    x.ArticleNumber,
                    x.Title,
                    x.AuthorId,
                    x.Status.Name,
                    x.PublishedAt,
                    x.ViewCount,
                    x.CreatedAt,
                    x.ModifiedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<GetArticlesByAuthorResponse>(items, totalCount, page, pageSize);
        }

        public async Task<GetArticleByIdResponse> GetByNumberAsync(int articleNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Select(x => new GetArticleByIdResponse(
                    x.ArticleNumber,
                    x.Title,
                    x.Content,
                    x.AuthorId,
                    x.Status.Name,
                    x.PublishedAt,
                    x.ViewCount + 1,
                    x.CreatedAt,
                    x.ModifiedAt,
                    x.Comments.Select(y => new CommentDto
                    {
                        Id = y.Id,
                        AuthorId = y.AuthorId,
                        Content = y.Content,
                        CreatedAt = y.CreatedAt,
                        ModifiedAt = y.ModifiedAt
                    }).ToList()))
                .FirstOrDefaultAsync(x => x.ArticleNumber == articleNumber, cancellationToken);
        }
    }
}
