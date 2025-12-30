using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Add
{
    public class AddServiceOfferingCommand : IRequest<Result<ServiceOfferingId>>
    {
        public Guid TrainerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int DurationMinutes { get; set; }
        public string ServiceType { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? StartDate {  get; set; }
        public DateTime? EndDate { get; set; }

        internal class AddServiceOfferingCommandHandler : IRequestHandler<AddServiceOfferingCommand, Result<ServiceOfferingId>>
        {
            private readonly ITrainerProfileRepository _trainerRepository;
            private readonly IUnitOfWork _unitOfWork;

            public AddServiceOfferingCommandHandler(
                ITrainerProfileRepository trainerRepository,
                IUnitOfWork unitOfWork)
            {
                _trainerRepository = trainerRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<ServiceOfferingId>> Handle(AddServiceOfferingCommand request, CancellationToken cancellationToken)
            {
                var trainerProfile = await _trainerRepository.GetWithServicesByIdAsync(request.TrainerId, cancellationToken);
                if (trainerProfile == null)
                {
                    return Result.UnprocessableEntity<ServiceOfferingId>(TrainerProfileNotFound);
                }

                var money = Money.Create(request.Price, request.Currency);
                var serviceType = Enumeration.FromDisplayName<ServiceCategory>(request.ServiceType);

                var serviceOffering = trainerProfile.AddService(
                    request.Name,
                    request.Description,
                    money,
                    request.DurationMinutes,
                    serviceType,
                    true,
                    request.MaxParticipants,
                    request.IsOnline,
                    request.StartDate,
                    request.EndDate);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(serviceOffering);
            }
        }
    }
}
