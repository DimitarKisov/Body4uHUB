using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Services.Infrastructure.Repositories
{
    internal class TrainerProfileRepository : ITrainerProfileRepository
    {
        private readonly ServicesDbContext _dbContext;

        public TrainerProfileRepository(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(TrainerProfile profile)
        {
            _dbContext.TrainerProfiles.Add(profile);
        }

        public async Task<bool> ExistsByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles.AnyAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task<TrainerProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TrainerProfile> GetWithServicesByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.TrainerProfiles
                .Include(x => x.Services)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
