using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using MediatR;

namespace Body4uHUB.Identity.Application.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
        internal class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
        {
            private readonly IRoleReadRepository _roleReadRepository;

            public GetAllRolesQueryHandler(IRoleReadRepository roleReadRepository)
            {
                _roleReadRepository = roleReadRepository;
            }

            public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
            {
                return await _roleReadRepository.GetAllAsync(cancellationToken);
            }
        }
    }
}
