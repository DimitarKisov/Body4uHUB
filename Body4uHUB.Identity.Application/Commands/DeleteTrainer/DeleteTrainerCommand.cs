using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;

namespace Body4uHUB.Identity.Application.Commands.DeleteTrainer
{
    public class DeleteTrainerCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }

        internal class DeleteTrainerCommandHandler : IRequestHandler<DeleteTrainerCommand, Result>
        {
            private readonly IUserRepository _userRepository;
            private readonly IRoleRepository _roleRepository;
            private readonly IEventBus _eventBus;

            public DeleteTrainerCommandHandler(
                IUserRepository userRepository,
                IRoleRepository roleRepository,
                IEventBus eventBus)
            {
                _userRepository = userRepository;
                _roleRepository = roleRepository;
                _eventBus = eventBus;
            }

            public async Task<Result> Handle(DeleteTrainerCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return Result.UnprocessableEntity(UserNotFound);
                }

                var role = await _roleRepository.FindByNameAsync("Trainer", cancellationToken);
                if (role == null)
                {
                    return Result.UnprocessableEntity(RoleNotFound);
                }

                var userIsInRole = user.Roles.Any(x => x.Id == role.Id);
                if (!userIsInRole)
                {
                    return Result.UnprocessableEntity(UserNotInRole);
                }

                var @event = new TrainerAccountDeletedEvent
                {
                    UserId = user.Id
                };

                await _eventBus.PublishAsync(@event);

                return Result.Success();
            }
        }
    }
}
