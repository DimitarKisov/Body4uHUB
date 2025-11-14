namespace Body4uHUB.Content.Api.Extensions
{
    using Body4uHUB.Shared.Exceptions;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("User ID not found in token");
            }

            return Guid.Parse(userIdClaim);
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value
                ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
        }

        public static string GetUserRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}