using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllUsers;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Shared.Application;
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

        public async Task<PagedResult<GetAllUsersResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _dbContext.Users.CountAsync(cancellationToken);

            var items = await _dbContext.Users
                .OrderBy(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllUsersResponse(
                    x.Id,
                    x.ContactInfo.Email,
                    x.FirstName,
                    x.LastName,
                    x.ContactInfo.PhoneNumber,
                    x.CreatedAt,
                    x.IsEmailConfirmed))
                .ToListAsync(cancellationToken);

            return new PagedResult<GetAllUsersResponse>(items, totalCount, page, pageSize);
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
