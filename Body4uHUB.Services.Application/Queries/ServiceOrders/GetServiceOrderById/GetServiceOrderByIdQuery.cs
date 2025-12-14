using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Queries.ServiceOrders.GetServiceOrderById
{
    public class GetServiceOrderByIdQuery : IRequest<Result<ServiceOrderDto>>
    {
        public int Id { get; set; }

        internal class GetServiceOrderByIdQueryHandler : IRequestHandler<GetServiceOrderByIdQuery, Result<ServiceOrderDto>>
        {
            private readonly IServiceOrderReadRepository _serviceOrderReadRepository;

            public GetServiceOrderByIdQueryHandler(IServiceOrderReadRepository serviceOrderReadRepository)
            {
                _serviceOrderReadRepository = serviceOrderReadRepository;
            }

            public async Task<Result<ServiceOrderDto>> Handle(GetServiceOrderByIdQuery request, CancellationToken cancellationToken)
            {
                var serviceOrder = await _serviceOrderReadRepository.GetByIdAsync(ServiceOrderId.Create(request.Id), cancellationToken);
                if (serviceOrder == null)
                {
                    return Result.UnprocessableEntity<ServiceOrderDto>(ServiceOrderNotFound);
                }

                return Result.Success(serviceOrder);
            }
        }
    }
}
