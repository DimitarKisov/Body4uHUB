using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using MediatR;

namespace Body4uHUB.Identity.Application.Commands.Register
{
    public class RegisterCommand : IRequest<RegisterResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
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

            public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var userExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
                if (userExists)
                {
                    throw new InvalidOperationException("User with the given email already exists.");
                }

                var passwordHash = _passwordHasherService.HashPassword(request.Password);

                var user = User.Create(
                    passwordHash,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber);

                _userRepository.Add(user);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var token = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, string.Empty);

                return new RegisterResponseDto
                {
                    AccessToken = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.ContactInfo.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.ContactInfo.PhoneNumber
                    }
                };
            }
        }
    }
}
