using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Identity.Application.Queries.GetAllRoles
{
    public record GetAllRolesQuery() : IRequest<Result<IEnumerable<RoleDto>>>;

    internal sealed class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<RoleDto>>>
    {
        private readonly IRoleReadRepository _roleReadRepository;

        public GetAllRolesQueryHandler(IRoleReadRepository roleReadRepository)
        {
            _roleReadRepository = roleReadRepository;
        }

        public async Task<Result<IEnumerable<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleReadRepository.GetAllAsync(cancellationToken);

            return Result.Success(roles);
        }
    }
}
