using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Application;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPasswordHasherService _passwordHasherService;
            private readonly IUnitOfWork _unitOfWork;

            public ChangePasswordCommandHandler(
                IUserRepository userRepository,
                IPasswordHasherService passwordHasherService,
                IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _passwordHasherService = passwordHasherService;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return Result.UnprocessableEntity(UserNotFound);
                }

                if (!_passwordHasherService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                {
                    return Result.Unauthorized(InvalidCredentials);
                }

                var newPasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
                if (string.IsNullOrWhiteSpace(newPasswordHash))
                {
                    return Result.UnprocessableEntity(PasswordInvalid);
                }

                user.UpdatePasswordHash(newPasswordHash);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
