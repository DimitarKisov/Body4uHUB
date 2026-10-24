using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Shared.Application;
using MediatR;

namespace Body4uHUB.Identity.Application.Queries.GetPublicUsers
{
    public record GetPublicUsersQuery(IReadOnlyCollection<Guid> Ids) : IRequest<Result<IReadOnlyList<PublicUserResponse>>>;

    internal sealed class GetPublicUsersQueryHandler(
        IUserReadRepository userReadRepository)
        : IRequestHandler<GetPublicUsersQuery, Result<IReadOnlyList<PublicUserResponse>>>
    {
        private readonly IUserReadRepository _userReadRepository = userReadRepository;

        // Unknown ids are simply missing from the result, so a page with one removed author still renders.
        public async Task<Result<IReadOnlyList<PublicUserResponse>>> Handle(GetPublicUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userReadRepository.GetPublicByIdsAsync([.. request.Ids.Distinct()], cancellationToken);

            return Result.Success(users);
        }
    }
}
