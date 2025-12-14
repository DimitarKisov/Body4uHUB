using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface IServiceOrderRepository : IRepository<ServiceOrder>
    {
        void Add(ServiceOrder serviceOrder);
        Task<ServiceOrder> GetByIdAsync(ServiceOrderId id, CancellationToken cancellationToken = default);
    }
}
