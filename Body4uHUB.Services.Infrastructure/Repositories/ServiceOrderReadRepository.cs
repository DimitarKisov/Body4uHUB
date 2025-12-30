using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Services.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Services.Infrastructure.Repositories
{
    internal class ServiceOrderReadRepository : IServiceOrderReadRepository
    {
        private readonly ServicesDbContext _dbContext;

        public ServiceOrderReadRepository(ServicesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ServiceOrderDto>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ServiceOrders
                .Where(x => x.ClientId == clientId)
                .Select(x => new ServiceOrderDto
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ServiceOfferingId = x.ServiceOfferingId,
                    TotalPrice = x.TotalPrice.Amount,
                    Currency = x.TotalPrice.Currency,
                    Status = x.Status.ToString(),
                    OrderDate = x.CreatedAt,
                    CompletedDate = x.CompletedAt,
                    Notes = x.Notes,
                    Review = x.Review != null ? new ReviewDto
                    {
                        Rating = x.Review.Rating,
                        Comment = x.Review.Comment,
                        CreatedAt = x.Review.CreatedAt
                    } : null
                })
                .ToListAsync(cancellationToken);
        }

        public Task<ServiceOrderDto> GetByIdAsync(ServiceOrderId id, CancellationToken cancellationToken = default)
        {
            return _dbContext.ServiceOrders
                .Where(x => x.Id == id)
                .Select(x => new ServiceOrderDto
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ServiceOfferingId = x.ServiceOfferingId,
                    TotalPrice = x.TotalPrice.Amount,
                    Currency = x.TotalPrice.Currency,
                    Status = x.Status.ToString(),
                    OrderDate = x.CreatedAt,
                    CompletedDate = x.CompletedAt,
                    Notes = x.Notes,
                    Review = x.Review != null ? new ReviewDto
                    {
                        Rating = x.Review.Rating,
                        Comment = x.Review.Comment,
                        CreatedAt = x.Review.CreatedAt
                    } : null
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
