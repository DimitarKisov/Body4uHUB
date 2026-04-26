using BCrypt.Net;
using Body4uHUB.Identity.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Body4uHUB.Identity.Infrastructure.Services
{
    /// <summary>
    /// Password Hasher Service implementation using BCrypt
    /// </summary>
    internal class PasswordHasherService : IPasswordHasherService
    {
        private readonly int _workFactor;
        private readonly ILogger<PasswordHasherService> _logger;

        public PasswordHasherService(
            IConfiguration configuration,
            ILogger<PasswordHasherService> logger)
        {
            _workFactor = configuration.GetValue<int>("Security:Hashing:WorkFactor", defaultValue: 12);
            _logger = logger;
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch (SaltParseException ex)
            {
                _logger.LogWarning(ex, "BCrypt verification failed due to invalid hash format.");
                return false;
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
            {
                _logger.LogError(ex, "Unexpected error during password verification.");
                return false;
            }
        }
    }
}