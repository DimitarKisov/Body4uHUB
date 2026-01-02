using Body4uHUB.Services.Domain.Models;
using Body4uHUB.Services.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain;
using MassTransit;

namespace Body4uHUB.Services.Infrastructure.Messaging.Consumers
{
    internal class TrainerAccountCreatedEventConsumer : IConsumer<TrainerAccountCreatedEvent>
    {
        private readonly ITrainerProfileRepository _trainerProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TrainerAccountCreatedEventConsumer(
            ITrainerProfileRepository trainerProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _trainerProfileRepository = trainerProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<TrainerAccountCreatedEvent> context)
        {
            var message = context.Message;

            try
            {
                var trainerProfileExists = await _trainerProfileRepository.ExistsByUserId(message.UserId);
                if (trainerProfileExists)
                {
                    //LOG INFO
                    return;
                }

                var trainerProfile = TrainerProfile.Create(message.UserId, message.Bio, message.YearsOfExperience);
                _trainerProfileRepository.Add(trainerProfile);

                await _unitOfWork.SaveChangesAsync();

                //LOG INFO
            }
            catch (Exception ex)
            {
                //LOG ERROR
            }
        }
    }
}
