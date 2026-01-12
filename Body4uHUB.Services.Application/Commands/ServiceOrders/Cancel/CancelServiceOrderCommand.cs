using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Cancel
{
    public class CancelServiceOrderCommand : IRequest<Result>
    {
        public int Id { get; set; }

        public AuthorizationContext AuthContext { get; set; }

        internal class CancelServiceOrderCommandHandler : IRequestHandler<CancelServiceOrderCommand, Result>
        {
            private readonly IServiceOrderRepository _serviceOrderRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CancelServiceOrderCommandHandler(
                IServiceOrderRepository serviceOrderRepository,
                IUnitOfWork unitOfWork)
            {
                _serviceOrderRepository = serviceOrderRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CancelServiceOrderCommand request, CancellationToken cancellationToken)
            {
                var serviceOrder = await _serviceOrderRepository.GetByIdAsync(ServiceOrderId.Create(request.Id), cancellationToken);
                if (serviceOrder == null)
                {
                    return Result.ResourceNotFound(ServiceOrderNotFound);
                }

                if (!request.AuthContext.IsAdmin && serviceOrder.ClientId != request.AuthContext.CurrentUserId)
                {
                    return Result.Forbidden(ServiceOrderForbidden);
                }

                serviceOrder.Cancel();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
