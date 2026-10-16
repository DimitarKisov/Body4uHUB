using Body4uHUB.Services.Domain.Enumerations;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Domain.Enumerations;
using MediatR;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOfferings.Add
{
    public record AddServiceOfferingCommand(
        Guid TrainerId,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int DurationMinutes,
        string ServiceType,
        int MaxParticipants,
        bool IsOnline,
        DateTime? StartDate,
        DateTime? EndDate,
        AuthorizationContext AuthContext)
        : IRequest<Result<AddServiceOfferingResponse>>;

    internal sealed class AddServiceOfferingCommandHandler(
        ITrainerProfileRepository trainerRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<AddServiceOfferingCommand, Result<AddServiceOfferingResponse>>
    {
        private readonly ITrainerProfileRepository _trainerRepository = trainerRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AddServiceOfferingResponse>> Handle(AddServiceOfferingCommand request, CancellationToken cancellationToken)
        {
            var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId, cancellationToken);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound<AddServiceOfferingResponse>(TrainerProfileNotFound);
            }

            var money = Money.Create(request.Price, request.Currency);
            var serviceType = Enumeration.FromDisplayName<ServiceCategory>(request.ServiceType);

            var serviceOfferingId = trainerProfile.AddService(
                request.Name,
                request.Description,
                money,
                request.DurationMinutes,
                serviceType,
                true,
                request.MaxParticipants,
                request.IsOnline,
                request.StartDate,
                request.EndDate,
                request.AuthContext.CurrentUserId,
                request.AuthContext.IsAdmin);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddServiceOfferingResponse(serviceOfferingId));
        }
    }
}
