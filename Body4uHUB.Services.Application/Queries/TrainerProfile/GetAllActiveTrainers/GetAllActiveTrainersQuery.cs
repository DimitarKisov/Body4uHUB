using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Services.Application.Queries.TrainerProfile.GetAllActiveTrainers
{
    public class GetAllActiveTrainersQuery : IRequest<Result<IEnumerable<TrainerProfileDto>>>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;

        internal class GetAllActiveTrainersQueryHandler : IRequestHandler<GetAllActiveTrainersQuery, Result<IEnumerable<TrainerProfileDto>>>
        {
            private readonly ITrainerProfileReadRepository _trainerReadRepository;

            public GetAllActiveTrainersQueryHandler(ITrainerProfileReadRepository trainerReadRepository)
            {
                _trainerReadRepository = trainerReadRepository;
            }

            public async Task<Result<IEnumerable<TrainerProfileDto>>> Handle(GetAllActiveTrainersQuery request, CancellationToken cancellationToken)
            {
                var trainerProfiles = await _trainerReadRepository.GetAllActiveAsync(request.Skip, request.Take, cancellationToken);

                return Result.Success(trainerProfiles);
            }
        }
    }
}
