using Body4uHUB.Services.Application.DTOs;
using Body4uHUB.Services.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Queries.ServiceOffering.GetServiceOfferingsByTrainer
{
    public class GetServiceOfferingsByTrainerQuery : IRequest<Result<IEnumerable<ServiceOfferingDto>>>
    {
        public Guid TrainerId { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;

        internal class GetServiceOfferingsByTrainerQueryHandler : IRequestHandler<GetServiceOfferingsByTrainerQuery, Result<IEnumerable<ServiceOfferingDto>>>
        {
            private readonly ITrainerProfileReadRepository _trainerReadRepository;

            public GetServiceOfferingsByTrainerQueryHandler(ITrainerProfileReadRepository trainerReadRepository)
            {
                _trainerReadRepository = trainerReadRepository;
            }

            public async Task<Result<IEnumerable<ServiceOfferingDto>>> Handle(GetServiceOfferingsByTrainerQuery request, CancellationToken cancellationToken)
            {
                var trainerExists = await _trainerReadRepository.ExistsByIdAsync(request.TrainerId);
                if (!trainerExists)
                {
                    return Result.ResourceNotFound<IEnumerable<ServiceOfferingDto>>(TrainerProfileNotFound);
                }

                var serviceOfferings = await _trainerReadRepository.GetServiceOfferingsByTrainerIdAsync(request.TrainerId, request.Skip, request.Take);

                return Result.Success(serviceOfferings);
            }
        }
    }
}
