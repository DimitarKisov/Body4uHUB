using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _dbContext;

        public RoleRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Role role)
        {
            _dbContext.Roles.Add(role);
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _dbContext.Roles.AnyAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<Role> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }
    }
}
