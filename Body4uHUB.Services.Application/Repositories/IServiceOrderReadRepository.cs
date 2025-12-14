using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Domain.ValueObjects;

namespace Body4uHUB.Services.Application.Repositories
{
    public interface IServiceOrderReadRepository
    {
        Task<ServiceOrderDto> GetByIdAsync(ServiceOrderId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceOrderDto>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}
