using BCrypt.Net;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Identity.Infrastructure.Services
{
    /// <summary>
    /// Password Hasher Service implementation using BCrypt
    /// </summary>
    internal sealed class PasswordHasherService(
        IOptions<PasswordHashingSettings> passwordHashingSettings,
        ILogger<PasswordHasherService> logger)
        : IPasswordHasherService
    {
        private readonly PasswordHashingSettings _passwordHashingSettings = passwordHashingSettings.Value;
        private readonly ILogger<PasswordHasherService> _logger = logger;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password, _passwordHashingSettings.WorkFactor);
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
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                _logger.LogError(ex, "Unexpected error during password verification.");
                return false;
            }
        }
    }
}