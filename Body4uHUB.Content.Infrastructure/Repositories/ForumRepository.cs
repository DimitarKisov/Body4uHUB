using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Content.Infrastructure.Repositories;

internal sealed class ForumRepository(ContentDbContext dbContext)
    : IForumRepository
{
    private readonly ContentDbContext _dbContext = dbContext;

    public void Add(ForumTopic forumTopic)
    {
        _dbContext.ForumTopics.Add(forumTopic);
    }

    public async Task<bool> ExistsByTitleAsync(string title, int? exceptTopicId = null, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ForumTopics
            .AnyAsync(x => x.Title == title && !x.IsDeleted && x.Id != exceptTopicId, cancellationToken);
    }

    // Posts are owned by the topic, so EF always loads all of them unless the include is filtered.
    // Topic.Posts then holds only what is needed for the operation, not the whole thread.
    public async Task<ForumTopic> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ForumTopics
            .Where(x => !x.IsDeleted)
            .Include(x => x.Posts
                .Where(y => false))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ForumTopic> GetByIdWithPostAsync(int id, int postId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ForumTopics
            .Where(x => !x.IsDeleted)
            .Include(x => x.Posts
                .Where(y => y.Id == postId && !y.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> IncrementViewCountAsync(int id, CancellationToken cancellationToken = default)
    {
        var affected = await _dbContext.ForumTopics
            .Where(x => x.Id == id && !x.IsDeleted)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.ViewCount, y => y.ViewCount + 1), cancellationToken);

        return affected > 0;
    }
}
