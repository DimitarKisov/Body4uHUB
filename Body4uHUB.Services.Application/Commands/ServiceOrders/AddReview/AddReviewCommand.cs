using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOrders.AddReview
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
                    return Result.UnprocessableEntity(ServiceOrderNotFound);
                }

                serviceOrder.AddReview(request.Rating, request.Comment, request.ClientId);

                var trainerProfile = await _trainerRepository.GetByIdAsync(serviceOrder.TrainerId);
                if (trainerProfile != null)
                {
                    trainerProfile.UpdateRating(request.Rating);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
