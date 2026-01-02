using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface ITrainerProfileRepository : IRepository<TrainerProfile>
    {
        void Add(TrainerProfile profile);
        Task<bool> ExistsByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task<TrainerProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TrainerProfile> GetWithServicesByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
