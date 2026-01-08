using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;
using System.Text.Json.Serialization;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Activate
{
    public class ActivateServiceOfferingCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public Guid TrainerId { get; set; }

        [JsonIgnore]
        public AuthorizationContext AuthContext { get; set; }

        internal class ActivateServiceOfferingCommandHandler : IRequestHandler<ActivateServiceOfferingCommand, Result>
        {
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public ActivateServiceOfferingCommandHandler(
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ActivateServiceOfferingCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetWithServicesByIdAsync(request.TrainerId, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.ResourceNotFound(TrainerProfileNotFound);
                }

                if (!request.AuthContext.IsAdmin && trainerProfile.Id != request.AuthContext.CurrentUserId)
                {
                    return Result.Forbidden(ServiceOfferingForbidden);
                }

                trainerProfile.ActivateService(ServiceOfferingId.Create(request.Id));

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
