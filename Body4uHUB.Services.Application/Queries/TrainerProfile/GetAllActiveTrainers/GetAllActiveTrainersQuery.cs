using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Services.Application.Queries.TrainerProfile.GetAllActiveTrainers
{
    public record GetAllActiveTrainersQuery(int Skip = 0, int Take = 20) : IRequest<Result<IEnumerable<TrainerProfileDto>>>;

    internal sealed class GetAllActiveTrainersQueryHandler : IRequestHandler<GetAllActiveTrainersQuery, Result<IEnumerable<TrainerProfileDto>>>
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
