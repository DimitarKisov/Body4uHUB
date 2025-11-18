using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Identity.Infrastructure.Persistance
{
    internal class DbInitializer : IDbInitializer
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            IdentityDbContext dbContext,
            IConfiguration configuration,
            IPasswordHasherService passwordHasherService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            ILogger<DbInitializer> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasherService = passwordHasherService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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
            var adminEmail = _configuration["SeedData:AdminUserEmail"];

            var adminExists = await _userRepository.ExistsByEmailAsync(adminEmail);
            if (adminExists)
            {
                return;
            }

            var password = _configuration["SeedData:AdminUserPassword"] ?? "SomeRandomPassword";
            var adminFirstName = _configuration["SeedData:AdminFirstName"] ?? "SomeRandomFirstName";
            var adminLastName = _configuration["SeedData:AdminLastName"] ?? "SomeRandomLastName";
            var adminRoleName = _configuration["SeedData:AdminRoleName"] ?? "Administrator";

            var hashedPassword = _passwordHasherService.HashPassword(password);

            var adminUser = User.Create(
                hashedPassword,
                adminFirstName,
                adminLastName,
                adminEmail,
                null);

            adminUser.ConfirmEmail();

            var adminRole = await _roleRepository.FindByNameAsync(adminRoleName);
            if (adminRole != null)
            {
                adminUser.AddRole(adminRole);
            }

            _userRepository.Add(adminUser);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var adminRoleName = _configuration["SeedData:AdminRoleName"] ?? "Administrator";
            var userRoles = _configuration.GetSection("SeedData:UserRoles").Get<List<string>>();

            var adminRoleExists = await _roleRepository.ExistsByNameAsync(adminRoleName);
            if (!adminRoleExists)
            {
                var adminRole = Role.Create(adminRoleName);
                _roleRepository.Add(adminRole);
            }

            foreach (var roleName in userRoles)
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
