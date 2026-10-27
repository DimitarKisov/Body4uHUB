using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.Logout
{
    /// <param name="RefreshToken">Raw value from the cookie; null or empty when there is none.</param>
    public record LogoutCommand(string RefreshToken) : IRequest<Result>;

    internal sealed class LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        // Always succeeds: signing out with a missing or already ended session leaves the client signed out either way.
        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result.Success();
            }

            var token = await _refreshTokenRepository.GetByHashAsync(RefreshToken.HashToken(request.RefreshToken), cancellationToken);
            if (token == null)
            {
                return Result.Success();
            }

            // The whole family: ends this device's session, including a copy rotated by someone else.
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            foreach (var active in await _refreshTokenRepository.GetActiveByFamilyAsync(token.FamilyId, now, cancellationToken))
            {
                active.Revoke(now);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
