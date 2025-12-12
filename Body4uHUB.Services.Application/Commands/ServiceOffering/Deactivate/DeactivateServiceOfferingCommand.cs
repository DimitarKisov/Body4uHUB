using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Deactivate
{
    public class DeactivateServiceOfferingCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public Guid TrainerId { get; set; }

        internal class DeactivateServiceOfferingCommandHandler : IRequestHandler<DeactivateServiceOfferingCommand, Result>
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
                    return Result.UnprocessableEntity(TrainerProfileNotFound);
                }

                trainerProfile.DeactivateService(ServiceOfferingId.Create(request.Id));

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
