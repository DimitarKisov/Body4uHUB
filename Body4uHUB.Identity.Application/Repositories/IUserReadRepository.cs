using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllUsers;
using Body4uHUB.Shared.Application;

namespace Body4uHUB.Identity.Application.Repositories
{
    public interface IUserReadRepository
    {
        Task<PagedResult<GetAllUsersResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
