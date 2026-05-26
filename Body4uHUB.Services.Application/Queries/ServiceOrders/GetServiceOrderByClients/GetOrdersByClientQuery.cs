using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderByClients
{
    public record GetOrdersByClientQuery(Guid ClientId) : IRequest<Result<IEnumerable<ServiceOrderDto>>>;

    internal sealed class GetOrdersByClientQueryHandler : IRequestHandler<GetOrdersByClientQuery, Result<IEnumerable<ServiceOrderDto>>>
    {
        private readonly IServiceOrderReadRepository _serviceOrderReadRepository;

        public GetOrdersByClientQueryHandler(IServiceOrderReadRepository serviceOrderReadRepository)
        {
            _serviceOrderReadRepository = serviceOrderReadRepository;
        }

        public async Task<Result<IEnumerable<ServiceOrderDto>>> Handle(GetOrdersByClientQuery request, CancellationToken cancellationToken)
        {
            var serviceOrders = await _serviceOrderReadRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

            return Result.Success(serviceOrders);
        }
    }
}
