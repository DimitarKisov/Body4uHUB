using Body4uHUB.Identity.Domain.Exceptions;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string Token) : IRequest<Result>;

    internal sealed class ConfirmEmailCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                // Throw the same exception as an invalid token so both produce an identical response
                // and the endpoint does not reveal which emails are registered.
                throw new InvalidUserException(EmailConfirmationTokenInvalid);
            }

            user.ConfirmEmail(request.Token);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
