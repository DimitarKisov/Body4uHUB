using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        internal class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IJwtTokenService _jwtTokenService;
            private readonly IPasswordHasherService _passwordHasherService;
            private readonly IUnitOfWork _unitOfWork;

            public LoginCommandHandler(
                IUserRepository userRepository,
                IJwtTokenService jwtTokenService,
                IPasswordHasherService passwordHasherService,
                IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _jwtTokenService = jwtTokenService;
                _passwordHasherService = passwordHasherService;
                _unitOfWork = unitOfWork;
            }

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

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.ContactInfo.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.ContactInfo.PhoneNumber,
                        CreatedAt = user.CreatedAt,
                        IsEmailConfirmed = user.IsEmailConfirmed,
                        Roles = user.Roles
                            .Select(r => new RoleDto
                            {
                                Id = r.Id,
                                Name = r.Name
                            })
                            .ToList()
                    }
                };

                return Result.Success(response);
            }
        }
    }
}