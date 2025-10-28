namespace Body4uHUB.Identity.Application.Services
{
    /// <summary>
    /// Service for password hashing and verification
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Hashes a plain text password
        /// </summary>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies if a plain text password matches a hashed password
        /// </summary>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
