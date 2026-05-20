using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class ForumReadRepository : IForumReadRepository
    {
        private readonly ContentDbContext _dbContext;

        public ForumReadRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<ForumTopicDto>> GetAllAsync(int page, int pageSize, bool includeDeleted, CancellationToken cancellationToken)
        {
            var query = _dbContext.ForumTopics
                .Where(x => includeDeleted || !x.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ForumTopicDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    AuthorId = x.AuthorId,
                    IsLocked = x.IsLocked,
                    ViewCount = x.ViewCount,
                    PostCount = x.Posts.Count(p => !p.IsDeleted),
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<ForumTopicDto>(items, totalCount, page, pageSize);
        }
    }
}
