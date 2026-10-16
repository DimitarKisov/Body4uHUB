using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Mappings;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Exceptions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PhoneNumber)
        : IRequest<Result<UserDto>>;

    internal sealed class RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasherService passwordHasherService,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<RegisterCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (userExists)
            {
                return Result.Conflict<UserDto>(UserEmailExists);
            }

            var passwordHash = _passwordHasherService.HashPassword(request.Password);

            var user = User.Create(
                passwordHash,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                _timeProvider.GetUtcNow().UtcDateTime);

            _userRepository.Add(user);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintViolationException)
            {
                // A concurrent registration with the same email passed the ExistsByEmailAsync check first.
                return Result.Conflict<UserDto>(UserEmailExists);
            }

            return Result.Success(user.ToDto([]));
        }
    }
}
