using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Confirm
{
    public record ConfirmServiceOrderCommand(int Id) : IRequest<Result>;

    internal sealed class ConfirmServiceOrderCommandHandler : IRequestHandler<ConfirmServiceOrderCommand, Result>
    {
        private readonly IServiceOrderRepository _serviceOrderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmServiceOrderCommandHandler(
            IServiceOrderRepository serviceOrderRepository,
            IUnitOfWork unitOfWork)
        {
            _serviceOrderRepository = serviceOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ConfirmServiceOrderCommand request, CancellationToken cancellationToken)
        {
            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(request.Id, cancellationToken);
            if (serviceOrder == null)
            {
                return Result.ResourceNotFound(ServiceOrderNotFound);
            }

            serviceOrder.Confirm();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
