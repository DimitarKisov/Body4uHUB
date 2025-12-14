using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using MediatR;

namespace Body4uHUB.Services.Application.Queries.TrainerProfile.GetAllActiveTrainers
{
    public class GetAllActiveTrainersQuery : IRequest<IEnumerable<TrainerProfileDto>>
    {
        internal class GetAllActiveTrainersQueryHandler : IRequestHandler<GetAllActiveTrainersQuery, IEnumerable<TrainerProfileDto>>
        {
            private readonly ITrainerProfileReadRepository _trainerReadRepository;

            public GetAllActiveTrainersQueryHandler(ITrainerProfileReadRepository trainerReadRepository)
            {
                _trainerReadRepository = trainerReadRepository;
            }

            public async Task<IEnumerable<TrainerProfileDto>> Handle(GetAllActiveTrainersQuery request, CancellationToken cancellationToken)
            {
                return await _trainerReadRepository.GetAllActiveAsync(cancellationToken);
            }
        }
    }
}
