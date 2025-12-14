using Body4uHUB.Services.Application.DTOs;

namespace Body4uHUB.Services.Application.Repositories
{
    public interface ITrainerProfileReadRepository
    {
        Task<TrainerProfileDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TrainerProfileDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    }
}
