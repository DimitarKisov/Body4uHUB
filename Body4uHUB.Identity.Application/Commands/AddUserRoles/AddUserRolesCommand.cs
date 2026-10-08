using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.AddUserRoles
{
    public record AddUserRolesCommand(Guid UserId, List<Guid> RoleIds) : IRequest<Result>;

    internal sealed class AddUserRolesCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<AddUserRolesCommand, Result>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

            foreach (var role in roles)
            {
                user.AddRole(role);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
