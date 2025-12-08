using Body4uHUB.Identity.Application.DTOs;

namespace Body4uHUB.Identity.Application.Repositories
{
    public interface IUserReadRepository
    {
        Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
