using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Domain.ValueObjects;
using Body4uHUB.Identity.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Body4uHUB.Identity.Infrastructure.Repositories
{
    internal sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
    {
        private readonly IdentityDbContext _dbContext = dbContext;

        public void Add(User user)
        {
            _dbContext.Users.Add(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            email = ContactInfo.NormalizeEmail(email);
            return await _dbContext.Users.AnyAsync(x => x.ContactInfo.Email == email, cancellationToken);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            email = ContactInfo.NormalizeEmail(email);
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.ContactInfo.Email == email, cancellationToken);
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
