using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared.Domain.Abstractions;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface IServiceOrderRepository : IRepository<ServiceOrder>
    {
        void Add(ServiceOrder serviceOrder);
        Task<ServiceOrder> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
