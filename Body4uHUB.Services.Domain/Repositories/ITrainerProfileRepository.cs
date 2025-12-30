using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface ITrainerProfileRepository : IRepository<TrainerProfile>
    {
        Task<TrainerProfile> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TrainerProfile> GetWithServicesByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
