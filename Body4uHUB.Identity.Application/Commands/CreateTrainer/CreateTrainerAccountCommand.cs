using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Application.Commands.CreateTrainer
{
    public class CreateTrainerAccountCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }

        internal class CreateTrainerAccountCommandHandler : IRequestHandler<CreateTrainerAccountCommand, Result>
        {
            private readonly IUserRepository _userRepository;
            private readonly IRoleRepository _roleRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IEventBus _eventBus;

            public CreateTrainerAccountCommandHandler(
                IUserRepository userRepository,
                IRoleRepository roleRepository,
                IUnitOfWork unitOfWork,
                IEventBus eventBus)
            {
                _roleRepository = roleRepository;
                _eventBus = eventBus;
            }

            public async Task<Result> Handle(CreateTrainerAccountCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return Result.ResourceNotFound(UserNotFound);
                }

                var role = await _roleRepository.FindByNameAsync("Trainer", cancellationToken);
                if (role == null)
                {
                    return Result.ResourceNotFound(RoleNotFound);
                }

                var userIsInRole = user.Roles.Any(x => x.Id == role.Id);
                if (!userIsInRole)
                {
                    return Result.ResourceNotFound(UserNotInRole);
                }

                var @event = new TrainerAccountCreatedEvent
                {
                    UserId = request.UserId,
                    Bio = request.Bio,
                    YearsOfExperience = request.YearsOfExperience
                };

                await _eventBus.PublishAsync(@event);

                return Result.Success();
            }
        }
    }
}
