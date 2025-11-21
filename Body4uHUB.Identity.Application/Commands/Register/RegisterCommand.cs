using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Register
{
    public class RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPasswordHasherService _passwordHasherService;
            private readonly IJwtTokenService _jwtTokenService;
            private readonly IUnitOfWork _unitOfWork;

            public RegisterCommandHandler(
                IUserRepository userRepository,
                IPasswordHasherService passwordHasherService,
                IJwtTokenService jwtTokenService,
                IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _passwordHasherService = passwordHasherService;
                _jwtTokenService = jwtTokenService;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var userExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
                if (userExists)
                {
                    return Result.Conflict<AuthResponseDto>(UserEmailExists);
                }

                var passwordHash = _passwordHasherService.HashPassword(request.Password);
                if (string.IsNullOrWhiteSpace(passwordHash))
                {
                    return Result.UnprocessableEntity<AuthResponseDto>(PasswordInvalid);
                }

                var user = User.Create(
                    passwordHash,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber);

                _userRepository.Add(user);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var token = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, string.Empty);

                var response = new AuthResponseDto
                {
                    AccessToken = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.ContactInfo.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.ContactInfo.PhoneNumber,
                        CreatedAt = user.CreatedAt,
                        IsEmailConfirmed = user.IsEmailConfirmed
                    }
                };

                return Result.Success(response);
            }
        }
    }
}