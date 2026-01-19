using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Infrastructure.Messaging.Consumers
{
    internal class TrainerAccountDeletedEventConsumer : IConsumer<TrainerAccountDeletedEvent>
    {
        private readonly ITrainerProfileRepository _trainerProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TrainerAccountDeletedEventConsumer> _logger;

        public TrainerAccountDeletedEventConsumer(
            ITrainerProfileRepository trainerProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<TrainerAccountDeletedEventConsumer> logger)
        {
            _trainerProfileRepository = trainerProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TrainerAccountDeletedEvent> context)
        {
            var message = context.Message;

            try
            {
                var trainerProfile = await _trainerProfileRepository.GetByIdAsync(message.UserId);
                if (trainerProfile == null)
                {
                    _logger.LogWarning(TrainerProfileNotFoundDetailed, message.UserId);
                    return;
                }

                trainerProfile.DeactivateProfile();
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(TrainerProfileDeactivatedDetailed, message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrainerAccountDeletedEventConsumer\\Consume -> Exception:");
                throw;
            }
        }
    }
}
