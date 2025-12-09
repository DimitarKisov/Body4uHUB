using Body4uHUB.Content.Domain.Models;
using Body4uHUB.Content.Domain.Repositories;
using Body4uHUB.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Body4uHUB.Content.Infrastructure.Repositories
{
    internal class ForumTopicRepository : IForumTopicRepository
    {
        private readonly ContentDbContext _dbContext;

        public ForumTopicRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ForumTopic forumTopic)
        {
            _dbContext.ForumTopics.Add(forumTopic);
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ForumTopics.AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ForumTopics.AnyAsync(x => x.Title == title, cancellationToken);
        }

        public async Task<ForumTopic> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ForumTopics
                .Include(x => x.Posts)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public void Remove(ForumTopic forumTopic)
        {
            _dbContext.ForumTopics.Remove(forumTopic);
        }
    }
}
