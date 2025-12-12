using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Update
{
    public class UpdateServiceOfferingCommand : IRequest<Result>
    {
        public ServiceOfferingId Id { get; set; }
        public Guid TrainerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int DurationMinutes { get; set; }
        public string ServiceType { get; set; }

        internal class UpdateServiceOfferingCommandHandler : IRequestHandler<UpdateServiceOfferingCommand, Result>
        {
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public UpdateServiceOfferingCommandHandler(
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdateServiceOfferingCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId);
                if (trainerProfile == null)
                {
                    return Result.UnprocessableEntity(TrainerProfileNotFound);
                }

                var serviceOffering = trainerProfile.GetService(request.Id);
                if (serviceOffering == null)
                {
                    return Result.UnprocessableEntity(ServiceOfferingNotFound);
                }

                var money = Money.Create(request.Price, serviceOffering.Price.Currency);

                serviceOffering.UpdateName(request.Name);
                serviceOffering.UpdateDescription(request.Description);
                serviceOffering.UpdatePrice(money);
                serviceOffering.UpdateDurationInMinutes(request.DurationMinutes);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
