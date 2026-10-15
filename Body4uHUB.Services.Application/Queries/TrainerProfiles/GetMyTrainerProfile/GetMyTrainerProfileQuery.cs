using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Queries.TrainerProfiles.GetMyTrainerProfile
{
    // Returns the current user's profile even when it is incomplete, so the trainer can find it and fill it in.
    public record GetMyTrainerProfileQuery(Guid UserId) : IRequest<Result<TrainerProfileDto>>;

    internal sealed class GetMyTrainerProfileQueryHandler(ITrainerProfileReadRepository trainerReadRepository)
        : IRequestHandler<GetMyTrainerProfileQuery, Result<TrainerProfileDto>>
    {
        private readonly ITrainerProfileReadRepository _trainerReadRepository = trainerReadRepository;

        public async Task<Result<TrainerProfileDto>> Handle(GetMyTrainerProfileQuery request, CancellationToken cancellationToken)
        {
            var trainerProfile = await _trainerReadRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound<TrainerProfileDto>(TrainerProfileNotFound);
            }

            return Result.Success(trainerProfile);
        }
    }
}
