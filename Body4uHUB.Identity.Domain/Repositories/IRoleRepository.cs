using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Identity.Domain.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        void Add(Role role);
        Task<bool> ExistByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Role> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
