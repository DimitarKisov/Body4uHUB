namespace Body4uHUB.Identity.Application.Services
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates JWT access token for a user
        /// </summary>
        /// <returns>JWT access token</returns>
        string GenerateAccessToken(Guid userId, string email, string role);

        /// <summary>
        /// Validates JWT token and returns user ID
        /// </summary>
        /// <returns>User ID if token is valid, null otherwise</returns>
        Guid? ValidateToken(string token);
    }
}
