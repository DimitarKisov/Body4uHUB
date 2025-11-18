using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared;
using Body4uHUB.Shared.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Body4uHUB.Identity.Infrastructure.Persistance
{
    internal class DbInitializer : IDbInitializer
    {
        private readonly IdentityDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DbInitializer(
            IdentityDbContext dbContext,
            IConfiguration configuration,
            IPasswordHasherService passwordHasherService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasherService = passwordHasherService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _dbContext.Database.MigrateAsync();
                await SeedRolesAsync();
                await SeedUsersAsync();
            }
            catch (Exception)
            {

                throw;
            }
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

            var hashedPassword = _passwordHasherService.HashPassword(password);

            var adminUser = User.Create(
                hashedPassword,
                adminFirstName,
                adminLastName,
                adminEmail,
                null);

            adminUser.ConfirmEmail();

            _userRepository.Add(adminUser);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var adminRoleName = _configuration["SeedData:AdminRoleName"];
            var userRoles = _configuration.GetSection("SeedData:UserRoles").Get<List<string>>();
            foreach (var roleName in userRoles)
            {
                // TODO
            }
        }
    }
}
