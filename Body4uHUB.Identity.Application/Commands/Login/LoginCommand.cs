using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Login
{
    /// <param name="RememberMe">Keep the session across browser restarts (persistent refresh cookie).</param>
    public record LoginCommand(string Email, string Password, bool RememberMe = false) : IRequest<Result<AuthSession>>;

    internal sealed class LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        AuthSessionFactory authSessionFactory,
        IPasswordHasherService passwordHasherService,
        IUnitOfWork unitOfWork,
        IOptions<RefreshTokenSettings> refreshTokenSettings,
        TimeProvider timeProvider)
        : IRequestHandler<LoginCommand, Result<AuthSession>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly AuthSessionFactory _authSessionFactory = authSessionFactory;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RefreshTokenSettings _refreshTokenSettings = refreshTokenSettings.Value;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result<AuthSession>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !_passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result.Unauthorized<AuthSession>(InvalidCredentials);
            }

            if (!user.IsEmailConfirmed)
            {
                return Result.Forbidden<AuthSession>(EmailNotConfirmed);
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;

            // Each sign-in starts its own session family, so signing in on another device does not end this one.
            var refreshToken = RefreshToken.Issue(
                user.Id, request.RememberMe, now, _refreshTokenSettings.Inactivity, _refreshTokenSettings.Lifetime, out var rawRefreshToken);
            _refreshTokenRepository.Add(refreshToken);

            user.UpdateLastLogin(now);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var session = await _authSessionFactory.CreateAsync(user, refreshToken, rawRefreshToken, cancellationToken);

            return Result.Success(session);
        }
    }
}