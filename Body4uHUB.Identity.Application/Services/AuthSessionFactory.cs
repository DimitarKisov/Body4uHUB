using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Mappings;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;

namespace Body4uHUB.Identity.Application.Services
{
    /// <summary>
    /// Builds the access token and response for login and refresh. Roles are read from the database every time,
    /// so a refreshed token carries the user's current roles.
    /// </summary>
    internal sealed class AuthSessionFactory(
        IRoleRepository roleRepository,
        IJwtTokenService jwtTokenService)
    {
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<AuthSession> CreateAsync(User user, RefreshToken refreshToken, string rawRefreshToken, CancellationToken cancellationToken)
        {
            var roleIds = user.Roles.Select(x => x.RoleId).ToList();
            var roles = await _roleRepository.FindByIdsAsync(roleIds, cancellationToken);

            var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, roles);
            var response = new AuthResponseDto(accessToken, user.ToDto(roles), refreshToken.FamilyExpiresAt);

            return new AuthSession(response, rawRefreshToken, refreshToken.ExpiresAt, refreshToken.IsPersistent);
        }
    }
}
