using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Queries.GetAllUsers;
using Body4uHUB.Identity.Application.Repositories;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Body4uHUB.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal sealed class UserReadRepository(IdentityDbContext dbContext) : IUserReadRepository
    {
        private readonly IdentityDbContext _dbContext = dbContext;

        public async Task<PagedResult<GetAllUsersResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _dbContext.Users.CountAsync(cancellationToken);

            var items = await _dbContext.Users
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id) // tie-breaker: equal timestamps must not move between pages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GetAllUsersResponse(
                    x.Id,
                    x.ContactInfo.Email,
                    x.FirstName,
                    x.LastName,
                    x.ContactInfo.PhoneNumber,
                    x.CreatedAt,
                    x.IsEmailConfirmed,
                    _dbContext.Roles
                        .Where(r => x.Roles.Any(ur => ur.RoleId == r.Id))
                        .Select(r => r.Name)
                        .ToList()))
                .ToListAsync(cancellationToken);

            return new PagedResult<GetAllUsersResponse>(items, totalCount, page, pageSize);
        }

        public async Task<UserDto> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserDto(
                    x.Id,
                    x.ContactInfo.Email,
                    x.FirstName,
                    x.LastName,
                    x.ContactInfo.PhoneNumber,
                    x.CreatedAt,
                    x.IsEmailConfirmed,
                    _dbContext.Roles
                        .Where(r => x.Roles.Any(ur => ur.RoleId == r.Id))
                        .Select(r => new RoleDto(r.Id, r.Name))
                        .ToList()))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
