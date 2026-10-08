using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.ResendEmailConfirmation
{
    public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;

    internal sealed class ResendEmailConfirmationCommandHandler(
        IUserRepository userRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ResendEmailConfirmationCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEventBus _eventBus = eventBus;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // Unknown and already confirmed emails succeed silently, so the endpoint does not reveal which emails are registered.
            if (user == null || user.IsEmailConfirmed)
            {
                return Result.Success();
            }

            user.RegenerateEmailConfirmationToken();

            // Published before SaveChanges so the outbox stores the message in the same transaction as the new token.
            await _eventBus.PublishAsync(new EmailConfirmationResendRequestedEvent { UserId = user.Id });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
