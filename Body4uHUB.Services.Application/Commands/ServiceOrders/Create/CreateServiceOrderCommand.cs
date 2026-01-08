using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.Create
{
    public class CreateServiceOrderCommand : IRequest<Result<ServiceOrderId>>
    {
        public Guid ClientId { get; set; }
        public Guid TrainerId { get; set; }
        public int ServiceOfferingId { get; set; }
        public string Notes { get; set; }

        internal class CreateServiceOrderCommandHandler : IRequestHandler<CreateServiceOrderCommand, Result<ServiceOrderId>>
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

            public async Task<Result<ServiceOrderId>> Handle(CreateServiceOrderCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetWithServicesByIdAsync(request.TrainerId, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.ResourceNotFound<ServiceOrderId>(TrainerProfileNotFound);
                }

                var serviceOffering = trainerProfile.GetService(Domain.ValueObjects.ServiceOfferingId.Create(request.ServiceOfferingId));
                if (serviceOffering == null)
                {
                    return Result.ResourceNotFound<ServiceOrderId>(ServiceOfferingNotFound);
                }

                if (!serviceOffering.IsActive)
                {
                    return Result.BusinessRuleViolation<ServiceOrderId>(ServiceOfferingInactive);
                }

                var serviceOrder = ServiceOrder.Create(
                    request.ClientId,
                    request.TrainerId,
                    Domain.ValueObjects.ServiceOfferingId.Create(request.ServiceOfferingId),
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
