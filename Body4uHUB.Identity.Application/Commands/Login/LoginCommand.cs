using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Mappings;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponseDto>>;

    internal sealed class LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasherService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !_passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result.Unauthorized<AuthResponseDto>(InvalidCredentials);
            }

            if (!user.IsEmailConfirmed)
            {
                return Result.Forbidden<AuthResponseDto>(EmailNotConfirmed);
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, user.Roles);

            user.UpdateLastLogin();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new AuthResponseDto(accessToken, user.ToDto());

            return Result.Success(response);
        }
    }
}