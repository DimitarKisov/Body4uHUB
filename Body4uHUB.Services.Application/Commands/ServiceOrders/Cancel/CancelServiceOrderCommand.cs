using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Cancel
{
    public record CancelServiceOrderCommand(int Id) : IRequest<Result>;

    internal sealed class CancelServiceOrderCommandHandler : IRequestHandler<CancelServiceOrderCommand, Result>
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
            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (serviceOrder == null)
            {
                return Result.ResourceNotFound(ServiceOrderNotFound);
            }

            serviceOrder.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
