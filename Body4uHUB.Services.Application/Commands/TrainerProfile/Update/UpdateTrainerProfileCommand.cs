using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Application.Commands.TrainerProfile.Update
{
    public class UpdateTrainerProfileCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public Guid CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }

        internal class UpdateTrainerProfileCommandHandler : IRequestHandler<UpdateTrainerProfileCommand, Result>
        {
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateTrainerProfileCommandHandler(
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdateTrainerProfileCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetByIdAsync(request.Id, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.UnprocessableEntity(TrainerProfileNotFound);
                }

                if (!request.IsAdmin && trainerProfile.Id != request.CurrentUserId)
                {
                    return Result.Forbidden(ServiceOfferingForbidden);
                }

                trainerProfile.UpdateBio(request.Bio);
                trainerProfile.UpdateYearsOfExperience(request.YearsOfExperience);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
