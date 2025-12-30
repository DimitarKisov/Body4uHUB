using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Services.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Services.Infrastructure.Repositories
{
    internal class ServiceOrderRepository : IServiceOrderRepository
    {
        private readonly ServicesDbContext _dbContext;

        public ServiceOrderRepository(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ServiceOrder serviceOrder)
        {
            _dbContext.ServiceOrders.Add(serviceOrder);
        }

        public async Task<ServiceOrder> GetByIdAsync(ServiceOrderId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ServiceOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
