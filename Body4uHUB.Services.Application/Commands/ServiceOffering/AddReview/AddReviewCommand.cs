using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.AddReview
{
    public class AddReviewCommand : IRequest<Result>
    {
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid ClientId { get; set; }

        internal class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Result>
        {
            private readonly IServiceOrderRepository _serviceOrderRepository;
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public AddReviewCommandHandler(
                IServiceOrderRepository serviceOrderRepository,
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _serviceOrderRepository = serviceOrderRepository;
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
            {
                var serviceOrder = await _serviceOrderRepository.GetByIdAsync(ServiceOrderId.Create(request.OrderId), cancellationToken);
                if (serviceOrder == null)
                {
                    return Result.ResourceNotFound(ServiceOrderNotFound);
                }

                var trainerProfile = await _trainerRepository.GetByIdAsync(serviceOrder.TrainerId);
                if (trainerProfile == null)
                {
                    return Result.ResourceNotFound(TrainerProfileNotFound);
                }

                var serviceOffering = trainerProfile.GetService(serviceOrder.ServiceOfferingId);
                if (serviceOffering == null)
                {
                    return Result.ResourceNotFound(ServiceOfferingNotFound);
                }

                serviceOffering.AddReview(request.ClientId, serviceOrder.Id, request.Rating, request.Comment);

                trainerProfile.UpdateRating();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
