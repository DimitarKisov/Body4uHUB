using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal class RoleReadRepository : IRoleReadRepository
    {
        private readonly IdentityDbContext _dbContext;

        public RoleReadRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles
                .Select(x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
