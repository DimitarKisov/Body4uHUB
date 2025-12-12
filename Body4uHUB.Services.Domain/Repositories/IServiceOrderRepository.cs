using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Services.Domain.Repositories
{
    public interface IServiceOrderRepository : IRepository<ServiceOrder>
    {
        void Add(ServiceOrder serviceOrder);
    }
}
