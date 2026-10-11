using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Identity.Infrastructure.Settings;
using Body4uHUB.Shared.Domain.Abstractions;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Identity.Infrastructure.Persistance
{
    internal sealed class DbInitializer(
        IdentityDbContext dbContext,
        IOptions<SeedDataSettings> seedDataSettings,
        IPasswordHasherService passwordHasherService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ILogger<DbInitializer> logger)
        : IDbInitializer
    {
        private readonly IdentityDbContext _dbContext = dbContext;
        private readonly SeedDataSettings _seedDataSettings = seedDataSettings.Value;
        private readonly IPasswordHasherService _passwordHasherService = passwordHasherService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DbInitializer> _logger = logger;

        public async Task InitializeAsync()
        {

            _logger.LogInformation("Starting database migration");
            await _dbContext.Database.MigrateAsync();
            _logger.LogInformation("Database migration completed");

            _logger.LogInformation("Starting seeding roles");
            await SeedRolesAsync();
            _logger.LogInformation("Seeding roles completed");

            _logger.LogInformation("Starting seeding admin user");
            await SeedUsersAsync();
            _logger.LogInformation("Seeding admin user completed");
        }

        private async Task SeedUsersAsync()
        {
            var adminExists = await _userRepository.ExistsByEmailAsync(_seedDataSettings.AdminUserEmail);
            if (adminExists)
            {
                return;
            }

            var hashedPassword = _passwordHasherService.HashPassword(_seedDataSettings.AdminUserPassword);

            var adminUser = User.Create(
                hashedPassword,
                _seedDataSettings.AdminUserFirstName,
                _seedDataSettings.AdminUserLastName,
                _seedDataSettings.AdminUserEmail,
                null);

            adminUser.ConfirmEmail(adminUser.EmailConfirmationToken);

            var adminRole = await _roleRepository.FindByNameAsync(_seedDataSettings.AdminRoleName);
            if (adminRole != null)
            {
                adminUser.AddRole(adminRole);
            }

            _userRepository.Add(adminUser);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var adminRoleExists = await _roleRepository.ExistsByNameAsync(_seedDataSettings.AdminRoleName);
            if (!adminRoleExists)
            {
                var adminRole = Role.Create(_seedDataSettings.AdminRoleName);
                _roleRepository.Add(adminRole);
            }

            foreach (var roleName in _seedDataSettings.UserRoles)
            {
                var roleExists = await _roleRepository.ExistsByNameAsync(roleName);
                if (roleExists)
                {
                    continue;
                }

                var role = Role.Create(roleName);
                _roleRepository.Add(role);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
