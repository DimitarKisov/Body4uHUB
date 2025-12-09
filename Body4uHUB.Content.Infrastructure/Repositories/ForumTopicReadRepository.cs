using Body4uHUB.Content.Application.DTOs;
using Body4uHUB.Content.Application.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class ForumTopicReadRepository : IForumTopicReadRepository
    {
        private readonly ContentDbContext _dbContext;

        public ForumTopicReadRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ForumTopicDto>> GetAllAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return await _dbContext.ForumTopics
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(take)
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
        }
    }
}
