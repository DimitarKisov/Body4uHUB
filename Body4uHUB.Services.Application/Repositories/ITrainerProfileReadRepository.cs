using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Services.Application.Repositories
{
    public interface ITrainerProfileReadRepository
    {
        Task<TrainerProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainerProfileDto>> GetAllActiveAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceOfferingDto>> GetServiceOfferingsByTrainerIdAsync(Guid trainerId, CancellationToken cancellationToken = default);
    }
}
