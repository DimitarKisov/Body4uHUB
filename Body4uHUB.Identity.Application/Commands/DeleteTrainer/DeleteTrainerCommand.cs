using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.DeleteTrainer
{
    public record DeleteTrainerCommand(Guid UserId) : IRequest<Result>;

    internal sealed class DeleteTrainerCommandHandler : IRequestHandler<DeleteTrainerCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrainerCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IEventBus eventBus,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteTrainerCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.ResourceNotFound(UserNotFound);
            }

            var trainerRole = await _roleRepository.FindByNameAsync(TrainerRoleName, cancellationToken);
            if (trainerRole == null)
            {
                return Result.ResourceNotFound(RoleNotFound);
            }

            user.EnsureIsTrainer(trainerRole);

            var @event = new TrainerAccountDeletedEvent
            {
                UserId = user.Id
            };

            await _eventBus.PublishAsync(@event);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
