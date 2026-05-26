using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Create
{
    public class CreateServiceOrderCommand : IRequest<Result<int>>
    {
        public Guid ClientId { get; set; }
        public Guid TrainerId { get; set; }
        public int ServiceOfferingId { get; set; }
        public string Notes { get; set; }

        internal class CreateServiceOrderCommandHandler : IRequestHandler<CreateServiceOrderCommand, Result<int>>
        {
            private readonly IServiceOrderRepository _serviceOrderRepository;
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public CreateServiceOrderCommandHandler(
                IServiceOrderRepository serviceOrderRepository,
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _serviceOrderRepository = serviceOrderRepository;
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<int>> Handle(CreateServiceOrderCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.ResourceNotFound<int>(TrainerProfileNotFound);
                }

                var serviceOffering = trainerProfile.GetService(request.ServiceOfferingId);
                if (serviceOffering == null)
                {
                    return Result.ResourceNotFound<int>(ServiceOfferingNotFound);
                }

                if (!serviceOffering.IsActive)
                {
                    return Result.BusinessRuleViolation<int>(ServiceOfferingInactive);
                }

                var serviceOrder = ServiceOrder.Create(
                    request.ClientId,
                    request.TrainerId,
                    request.ServiceOfferingId,
                    OrderStatus.Pending,
                    serviceOffering.Price,
                    PaymentStatus.Pending,
                    request.Notes
                );

                _serviceOrderRepository.Add(serviceOrder);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(serviceOrder.Id);
            }
        }
    }
}
