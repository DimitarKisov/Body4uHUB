using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Application.Commands.RefreshSession
{
    /// <param name="RefreshToken">Raw value from the cookie; null or empty when there is none.</param>
    public record RefreshSessionCommand(string RefreshToken) : IRequest<Result<AuthSession>>;

    internal sealed class RefreshSessionCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        AuthSessionFactory authSessionFactory,
        IUnitOfWork unitOfWork,
        IOptions<RefreshTokenSettings> refreshTokenSettings,
        TimeProvider timeProvider)
        : IRequestHandler<RefreshSessionCommand, Result<AuthSession>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly AuthSessionFactory _authSessionFactory = authSessionFactory;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RefreshTokenSettings _refreshTokenSettings = refreshTokenSettings.Value;
        private readonly TimeProvider _timeProvider = timeProvider;

        // Every failure is the same 401, so a caller cannot tell an unknown token from an expired or stolen one.
        public async Task<Result<AuthSession>> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Result.Unauthorized<AuthSession>(SessionInvalid);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var current = await _refreshTokenRepository.GetByHashAsync(RefreshToken.HashToken(request.RefreshToken), cancellationToken);
            if (current == null)
            {
                return Result.Unauthorized<AuthSession>(SessionInvalid);
            }

            if (!current.IsActive(now))
            {
                // An already rotated token came back: someone else holds a copy, so the whole session ends.
                if (current.WasRotated)
                {
                    await RevokeFamilyAsync(current.FamilyId, now, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return Result.Unauthorized<AuthSession>(SessionInvalid);
            }

            var user = await _userRepository.GetByIdAsync(current.UserId, cancellationToken);
            if (user == null)
            {
                current.Revoke(now);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Unauthorized<AuthSession>(SessionInvalid);
            }

            var next = current.Rotate(now, _refreshTokenSettings.Inactivity, out var rawRefreshToken);
            _refreshTokenRepository.Add(next);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var session = await _authSessionFactory.CreateAsync(user, next, rawRefreshToken, cancellationToken);

            return Result.Success(session);
        }

        private async Task RevokeFamilyAsync(Guid familyId, DateTime now, CancellationToken cancellationToken)
        {
            foreach (var token in await _refreshTokenRepository.GetActiveByFamilyAsync(familyId, now, cancellationToken))
            {
                token.Revoke(now);
            }
        }
    }
}
