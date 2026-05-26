using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.ServiceOffering.Update
{
    public record UpdateServiceOfferingCommand(
        int Id,
        Guid TrainerId,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int DurationMinutes,
        string ServiceType,
        AuthorizationContext AuthContext)
        : IRequest<Result>;

    internal sealed class UpdateServiceOfferingCommandHandler : IRequestHandler<UpdateServiceOfferingCommand, Result>
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
            var trainerProfile = await _trainerRepository.GetByIdAsync(request.TrainerId, cancellationToken);
            if (trainerProfile == null)
            {
                return Result.ResourceNotFound(TrainerProfileNotFound);
            }

            var serviceOffering = trainerProfile.GetService(request.Id);
            if (serviceOffering == null)
            {
                return Result.ResourceNotFound(ServiceOfferingNotFound);
            }

            if (!request.AuthContext.IsAdmin && trainerProfile.UserId != request.AuthContext.CurrentUserId)
            {
                return Result.Forbidden(ServiceOfferingForbidden);
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
