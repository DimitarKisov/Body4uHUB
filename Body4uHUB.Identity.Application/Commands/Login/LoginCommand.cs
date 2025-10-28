using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        internal class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
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

            public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
                if (user == null || !_passwordHasherService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    throw new InvalidOperationException("Invalid email or password.");
                }

                if (!user.IsEmailConfirmed)
                {
                    throw new InvalidOperationException("Email is not confirmed.");
                }

                var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, string.Empty);

                user.UpdateLastLogin();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new AuthResponseDto
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
                    }
                };
            }
        }
    }
}
