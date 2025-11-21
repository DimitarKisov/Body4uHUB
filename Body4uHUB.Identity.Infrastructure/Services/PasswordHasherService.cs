using Body4uHUB.Identity.Application.Services;

namespace Body4uHUB.Identity.Infrastructure.Services
{
    /// <summary>
    /// Password Hasher Service implementation using BCrypt
    /// </summary>
    internal class PasswordHasherService : IPasswordHasherService
    {
        private const int WorkFactor = 12; // BCrypt work factor (cost parameter)

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
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
            catch
            {
                // Invalid hash format or other BCrypt errors
                return false;
            }
        }
    }
}