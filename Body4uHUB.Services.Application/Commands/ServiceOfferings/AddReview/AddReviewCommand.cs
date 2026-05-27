using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOfferings.AddReview
{
    public record AddReviewCommand(
        Guid TrainerId,
        int ServiceId,
        int OrderId,
        int Rating,
        string Comment,
        AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, Result>
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
            var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound(TrainerProfileNotFound);
            }

            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (serviceOrder == null ||
                serviceOrder.TrainerId != request.TrainerId ||
                serviceOrder.ServiceOfferingId != request.ServiceId)
            {
                return Result.ResourceNotFound(ServiceOrderNotFound);
            }

            trainerProfile.AddReviewToService(
                request.ServiceId,
                request.AuthContext.CurrentUserId,
                serviceOrder.Id,
                request.Rating,
                request.Comment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
