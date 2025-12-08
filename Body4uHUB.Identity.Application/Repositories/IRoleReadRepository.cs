using Body4uHUB.Identity.Application.DTOs;

namespace Body4uHUB.Identity.Application.Repositories
{
    public interface IRoleReadRepository
    {
        Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
