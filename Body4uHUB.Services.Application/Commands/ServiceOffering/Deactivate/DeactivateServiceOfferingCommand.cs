using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Deactivate
{
    public record DeactivateServiceOfferingCommand(int Id, Guid TrainerId, AuthorizationContext AuthContext) : IRequest<Result>;

    internal sealed class DeactivateServiceOfferingCommandHandler : IRequestHandler<DeactivateServiceOfferingCommand, Result>
    {
        private readonly ITrainerProfileRepository _trainerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateServiceOfferingCommandHandler(
            ITrainerProfileRepository trainerRepository,
            IUnitOfWork unitOfWork)
        {
            _trainerRepository = trainerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeactivateServiceOfferingCommand request, CancellationToken cancellationToken)
        {
            var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId, cancellationToken);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound(TrainerProfileNotFound);
            }

            trainerProfile.DeactivateService(
                request.Id,
                request.AuthContext.CurrentUserId,
                request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
