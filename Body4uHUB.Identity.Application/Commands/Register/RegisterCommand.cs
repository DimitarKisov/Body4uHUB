using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Mappings;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
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
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RegisterCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IEventBus _eventBus = eventBus;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
                request.PhoneNumber);

            _userRepository.Add(user);

            // With the bus outbox, publishing only stages the message in the DbContext;
            // SaveChanges then stores the user and the message in one transaction.
            await _eventBus.PublishAsync(new UserRegisteredEvent { UserId = user.Id });

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto());
        }
    }
}
