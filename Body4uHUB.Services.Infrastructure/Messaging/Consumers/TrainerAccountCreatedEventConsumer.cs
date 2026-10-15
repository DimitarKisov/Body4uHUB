using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Infrastructure.Messaging.Consumers
{
    internal class TrainerAccountCreatedEventConsumer(
        ITrainerProfileRepository trainerProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<TrainerAccountCreatedEventConsumer> logger)
        : IConsumer<TrainerAccountCreatedEvent>
    {
        private readonly ITrainerProfileRepository _trainerProfileRepository = trainerProfileRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<TrainerAccountCreatedEventConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<TrainerAccountCreatedEvent> context)
        {
            var message = context.Message;

            try
            {
                var existingProfile = await _trainerProfileRepository.GetByUserIdAsync(message.UserId, context.CancellationToken);
                if (existingProfile != null)
                {
                    // An active profile means this message was already handled (redelivery).
                    if (existingProfile.IsActive)
                    {
                        _logger.LogInformation(string.Format(TrainerProfileAlreadyExists, message.UserId));
                        return;
                    }

                    existingProfile.Reactivate();
                    await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                    _logger.LogInformation(TrainerProfileReactivatedDetailed, message.UserId);
                    return;
                }

                var trainerProfile = TrainerProfile.Create(message.UserId);
                _trainerProfileRepository.Add(trainerProfile);

                await _unitOfWork.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation(string.Format(TrainerProfileCreatedSuccessfully, message.UserId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrainerAccountCreatedEventConsumer\\Consume -> Exception:");
                throw;
            }
        }
    }
}
