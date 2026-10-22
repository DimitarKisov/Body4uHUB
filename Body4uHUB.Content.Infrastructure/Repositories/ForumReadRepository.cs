using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Queries.Forum.GetById;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal sealed class ForumReadRepository(ContentDbContext dbContext)
        : IForumReadRepository
    {
        private readonly ContentDbContext _dbContext = dbContext;

        public async Task<PagedResult<ForumTopicDto>> GetAllAsync(int page, int pageSize, bool includeDeleted, CancellationToken cancellationToken)
        {
            var query = _dbContext.ForumTopics
                .Where(x => includeDeleted || !x.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ForumTopicDto(
                    x.Id,
                    x.Title,
                    x.AuthorId,
                    x.IsLocked,
                    x.ViewCount,
                    x.Posts.Count(p => !p.IsDeleted),
                    x.CreatedAt,
                    x.ModifiedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<ForumTopicDto>(items, totalCount, page, pageSize);
        }

        public async Task<GetForumTopicByIdResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbContext.ForumTopics
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new GetForumTopicByIdResponse(
                    x.Id,
                    x.Title,
                    x.AuthorId,
                    x.IsLocked,
                    x.ViewCount,
                    x.Posts.Count(y => !y.IsDeleted),
                    x.CreatedAt,
                    x.ModifiedAt,
                    x.Posts
                        .Where(y => !y.IsDeleted)
                        .OrderBy(y => y.CreatedAt)
                        .Select(y => new ForumPostDto(
                            y.Id,
                            y.Content,
                            y.AuthorId,
                            x.Id,
                            y.IsDeleted,
                            y.CreatedAt,
                            y.ModifiedAt))
                        .ToList()))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
