using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Identity.Application.Queries.GetAllUsers
{
    public record GetAllUsersQuery(int Page, int PageSize) : IRequest<Result<PagedResult<GetAllUsersResponse>>>;

    internal sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedResult<GetAllUsersResponse>>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetAllUsersQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<Result<PagedResult<GetAllUsersResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userReadRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            return Result.Success(users);
        }
    }
}
