using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Shared;

namespace Body4uHUB.Identity.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        void Add(User user);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
