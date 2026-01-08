using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain;
using MediatR;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.AddUserRoles
{
    public class AddUserRolesCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public List<Guid> RoleIds { get; set; }

        internal class AddUserRolesCommandHandler : IRequestHandler<AddUserRolesCommand, Result>
        {
            private readonly IUserRepository _userRepository;
            private readonly IRoleRepository _roleRepository;
            private readonly IUnitOfWork _unitOfWork;

            public AddUserRolesCommandHandler(
                IUserRepository userRepository,
                IRoleRepository roleRepository,
                IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _roleRepository = roleRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(AddUserRolesCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return Result.ResourceNotFound(UserNotFound);
                }

                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleRepository.FindByIdAsync(roleId, cancellationToken);
                    if (role == null)
                    {
                        return Result.ResourceNotFound($"Role '{roleId}' does not exist.");
                    }

                    user.AddRole(role);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
