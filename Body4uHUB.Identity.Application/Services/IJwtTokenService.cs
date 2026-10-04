using Body4uHUB.Identity.Domain.Models;

namespace Body4uHUB.Identity.Application.Services
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates JWT access token for a user
        /// </summary>
        /// <returns>JWT access token</returns>
        string GenerateAccessToken(Guid userId, string email, IReadOnlyCollection<Role> roles);
    }
}
