using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface ITrainerProfileRepository : IRepository<TrainerProfile>
    {
        void Add(TrainerProfile profile);
        Task<TrainerProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TrainerProfile> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
