using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Queries.TrainerProfile
{
    public class GetTrainerProfileByIdQuery : IRequest<Result<TrainerProfileDto>>
    {
        public Guid TrainerId { get; set; }

        internal class GetTrainerProfileByIdQueryHandler : IRequestHandler<GetTrainerProfileByIdQuery, Result<TrainerProfileDto>>
        {
            private readonly ITrainerProfileReadRepository _trainerReadRepository;

            public GetTrainerProfileByIdQueryHandler(ITrainerProfileReadRepository trainerReadRepository)
            {
                _trainerReadRepository = trainerReadRepository;
            }

            public async Task<Result<TrainerProfileDto>> Handle(GetTrainerProfileByIdQuery request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerReadRepository.GetByIdAsync(request.TrainerId, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.UnprocessableEntity<TrainerProfileDto>(TrainerProfileNotFound);
                }

                return Result.Success(trainerProfile);
            }
        }
    }
}
