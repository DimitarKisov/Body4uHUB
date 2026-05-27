using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOfferings.Update
{
    public record UpdateServiceOfferingCommand(
        int Id,
        Guid TrainerId,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int DurationMinutes,
        string ServiceType,
        AuthorizationContext AuthContext)
        : IRequest<Result>;

    internal sealed class UpdateServiceOfferingCommandHandler : IRequestHandler<UpdateServiceOfferingCommand, Result>
    {
        private readonly ITrainerProfileRepository _trainerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateServiceOfferingCommandHandler(
            ITrainerProfileRepository trainerRepository,
            IUnitOfWork unitOfWork)
        {
            _trainerRepository = trainerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateServiceOfferingCommand request, CancellationToken cancellationToken)
        {
            var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId, cancellationToken);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound(TrainerProfileNotFound);
            }

            trainerProfile.UpdateServiceDetails(
                request.Id,
                request.Name,
                request.Description,
                request.Price,
                request.DurationMinutes,
                request.AuthContext.CurrentUserId,
                request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
