using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.CreateTrainer
{
    public record CreateTrainerAccountCommand(Guid UserId) : IRequest<Result<CreateTrainerAccountResponse>>;

    internal sealed class CreateTrainerAccountCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<CreateTrainerAccountCommand, Result<CreateTrainerAccountResponse>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result<CreateTrainerAccountResponse>> Handle(CreateTrainerAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.ResourceNotFound<CreateTrainerAccountResponse>(UserNotFound);
            }

            var trainerRole = await _roleRepository.FindByNameAsync(TrainerRoleName, cancellationToken);
            if (trainerRole == null)
            {
                return Result.ResourceNotFound<CreateTrainerAccountResponse>(RoleNotFound);
            }

            if (user.HasRole(trainerRole.Id))
            {
                return Result.Conflict<CreateTrainerAccountResponse>(UserAlreadyTrainer);
            }

            user.AssignTrainerRole(trainerRole);

            // Roles live in the access token: sign the user out everywhere, so the next sign-in carries the new ones.
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, _timeProvider.GetUtcNow().UtcDateTime, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateTrainerAccountResponse(user.Id));
        }
    }
}
