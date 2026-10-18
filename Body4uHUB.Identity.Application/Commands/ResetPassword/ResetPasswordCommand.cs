using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ResetPassword
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;

    internal sealed class ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // Hash first, so known and unknown emails take about the same time.
            var newPasswordHash = _passwordHasherService.HashPassword(request.NewPassword);

            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                // Same exception as a bad token: an identical 422 that does not reveal which emails are registered.
                throw new InvalidUserException(PasswordResetTokenInvalid);
            }

            user.ResetPassword(request.Token, newPasswordHash, _timeProvider.GetUtcNow().UtcDateTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
