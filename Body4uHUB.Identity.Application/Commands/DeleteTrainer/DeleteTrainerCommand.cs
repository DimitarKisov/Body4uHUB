using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.DeleteTrainer
{
    public record DeleteTrainerCommand(Guid UserId) : IRequest<Result>;

    internal sealed class DeleteTrainerCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<DeleteTrainerCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(DeleteTrainerCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.ResourceNotFound(UserNotFound);
            }

            var trainerRole = await _roleRepository.FindByNameAsync(TrainerRoleName, cancellationToken);
            if (trainerRole == null)
            {
                return Result.ResourceNotFound(RoleNotFound);
            }

            user.RevokeTrainerRole(trainerRole);

            // Roles live in the access token: sign the user out everywhere, so the next sign-in carries the new ones.
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, _timeProvider.GetUtcNow().UtcDateTime, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
