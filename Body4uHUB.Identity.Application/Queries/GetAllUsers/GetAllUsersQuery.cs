using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using MediatR;

namespace Body4uHUB.Identity.Application.Queries.GetAllUsers
{
    public record GetAllUsersQuery() : IRequest<IEnumerable<UserDto>>;

    internal sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetAllUsersQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _userReadRepository.GetAllAsync(cancellationToken);
        }
    }
}
