using Body4uHUB.Services.Application.DTOs;

namespace Body4uHUB.Services.Application.Repositories
{
    public interface ITrainerProfileReadRepository
    {
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TrainerProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainerProfileDto>> GetAllActiveAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceOfferingDto>> GetServiceOfferingsByTrainerIdAsync(Guid trainerId, int skip, int take, CancellationToken cancellationToken = default);
    }
}
