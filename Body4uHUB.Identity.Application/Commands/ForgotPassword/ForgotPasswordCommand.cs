using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;

    internal sealed class ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // Unknown emails succeed silently, so the endpoint does not reveal which emails are registered.
            if (user == null)
            {
                return Result.Success();
            }

            user.RequestPasswordReset(_timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
