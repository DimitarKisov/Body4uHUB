using Body4uHUB.Services.Application.DTOs;

namespace Body4uHUB.Services.Application.Repositories
{
    public interface IServiceOrderReadRepository
    {
        Task<ServiceOrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceOrderDto>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}
