using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.ResendEmailConfirmation
{
    public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;

    internal sealed class ResendEmailConfirmationCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<ResendEmailConfirmationCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // Unknown and already confirmed emails succeed silently, so the endpoint does not reveal which emails are registered.
            if (user == null || user.IsEmailConfirmed)
            {
                return Result.Success();
            }

            user.RegenerateEmailConfirmationToken(_timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
