using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ChangePassword
{
    /// <param name="CurrentRefreshToken">The caller's refresh cookie, so this session survives while every other one ends.</param>
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword, string CurrentRefreshToken = null) : IRequest<Result>;

    internal sealed class ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.ResourceNotFound(UserNotFound);
            }

            if (!_passwordHasherService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                // 422, not 401: the caller is authenticated; a 401 would make clients drop the session.
                return Result.BusinessRuleViolation(CurrentPasswordInvalid);
            }

            var newPasswordHash = _passwordHasherService.HashPassword(request.NewPassword);

            user.UpdatePasswordHash(newPasswordHash);

            // Other devices may be signed in by whoever knew the old password.
            var keepTokenHash = string.IsNullOrWhiteSpace(request.CurrentRefreshToken) ? null : RefreshToken.HashToken(request.CurrentRefreshToken);
            await _refreshTokenRepository.RevokeAllForUserAsync(
                user.Id, _timeProvider.GetUtcNow().UtcDateTime, cancellationToken, keepTokenHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
