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
    internal sealed class ArticleReadRepository(ContentDbContext dbContext) : IArticleReadRepository
    {
        private readonly ContentDbContext _dbContext = dbContext;

        public async Task<PagedResult<GetAllArticlesResponse>> GetAllArticlesAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _dbContext.Articles.Where(x => !x.IsDeleted && x.Status == ArticleStatus.Published);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllArticlesResponse(
                    x.Id,
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

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetArticlesByAuthorResponse(
                    x.Id,
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

        public async Task<GetArticleByIdResponse> GetWithCommentsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Articles
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new GetArticleByIdResponse(
                    x.Id,
                    x.Title,
                    x.Content,
                    x.AuthorId,
                    x.Status.Name,
                    x.PublishedAt,
                    x.ViewCount,
                    x.CreatedAt,
                    x.ModifiedAt,
                    x.Comments
                        .OrderBy(y => y.CreatedAt)
                        .Select(y => new CommentDto
                        {
                            Id = y.Id,
                            AuthorId = y.AuthorId,
                            Content = y.Content,
                            ParentCommentId = y.ParentCommentId,
                            IsDeleted = y.IsDeleted,
                            CreatedAt = y.CreatedAt,
                            ModifiedAt = y.ModifiedAt
                        }).ToList()))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
