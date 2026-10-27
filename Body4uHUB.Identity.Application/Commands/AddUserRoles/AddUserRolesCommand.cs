using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RoleConstants;
using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.AddUserRoles
{
    public record AddUserRolesCommand(Guid UserId, List<Guid> RoleIds) : IRequest<Result>;

    internal sealed class AddUserRolesCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
        : IRequestHandler<AddUserRolesCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result> Handle(AddUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result.ResourceNotFound(UserNotFound);
            }

            var roleIds = request.RoleIds.Distinct().ToList();
            var roles = await _roleRepository.FindByIdsAsync(roleIds, cancellationToken);

            var missingRoleIds = roleIds.Except(roles.Select(r => r.Id)).ToList();
            if (missingRoleIds.Count > 0)
            {
                return Result.ResourceNotFound($"Roles do not exist: {string.Join(", ", missingRoleIds)}.");
            }

            if (roles.Any(r => r.Name == TrainerRoleName))
            {
                return Result.BusinessRuleViolation(TrainerRoleRequiresTrainerAccount);
            }

            foreach (var role in roles)
            {
                user.AddRole(role);
            }

            // Roles live in the access token: sign the user out everywhere, so the next sign-in carries the new ones.
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, _timeProvider.GetUtcNow().UtcDateTime, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
