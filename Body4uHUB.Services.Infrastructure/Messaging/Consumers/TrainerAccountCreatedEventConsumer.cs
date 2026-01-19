using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Infrastructure.Messaging.Consumers
{
    internal class TrainerAccountCreatedEventConsumer : IConsumer<TrainerAccountCreatedEvent>
    {
        private readonly ITrainerProfileRepository _trainerProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TrainerAccountCreatedEventConsumer> _logger;

        public TrainerAccountCreatedEventConsumer(
            ITrainerProfileRepository trainerProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<TrainerAccountCreatedEventConsumer> logger)
        {
            _trainerProfileRepository = trainerProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TrainerAccountCreatedEvent> context)
        {
            var message = context.Message;

            try
            {
                var trainerProfileExists = await _trainerProfileRepository.ExistsByUserId(message.UserId);
                if (trainerProfileExists)
                {
                    _logger.LogInformation(string.Format(TrainerProfileAlreadyExists, message.UserId));
                    return;
                }

                var trainerProfile = TrainerProfile.Create(message.UserId, message.Bio, message.YearsOfExperience);
                _trainerProfileRepository.Add(trainerProfile);

                await _unitOfWork.SaveChangesAsync();

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
