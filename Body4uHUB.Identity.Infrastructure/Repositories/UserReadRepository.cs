using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal class UserReadRepository : IUserReadRepository
    {
        private readonly IdentityDbContext _dbContext;

        public UserReadRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
               .OrderBy(x => x.CreatedAt)
               .Select(x => new UserDto
               {
                   Id = x.Id,
                   Email = x.ContactInfo.Email,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   PhoneNumber = x.ContactInfo.PhoneNumber,
                   CreatedAt = x.CreatedAt,
                   IsEmailConfirmed = x.IsEmailConfirmed,
                   Roles = x.Roles.Select(y => new RoleDto
                   {
                       Id = y.Id,
                       Name = y.Name
                   }).ToList()
               })
               .ToListAsync(cancellationToken);
        }

        public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Email = x.ContactInfo.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.ContactInfo.PhoneNumber,
                    CreatedAt = x.CreatedAt,
                    IsEmailConfirmed = x.IsEmailConfirmed,
                    Roles = x.Roles.Select(y => new RoleDto
                    {
                        Id = y.Id,
                        Name = y.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
