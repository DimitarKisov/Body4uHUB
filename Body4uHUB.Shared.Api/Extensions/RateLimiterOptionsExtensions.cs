using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace Body4uHUB.Shared.Api.Extensions
{
    public static class RateLimiterOptionsExtensions
    {
        public static void AddPerIpFixedWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
        {
            options.AddPolicy(policyName, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = window,
                        QueueLimit = 0
                    }));
        }

        // For authenticated endpoints (UseRateLimiter runs after UseAuthentication); falls back to the IP without a user id.
        public static void AddPerUserFixedWindowPolicy(this RateLimiterOptions options, string policyName, int permitLimit, TimeSpan window)
        {
            options.AddPolicy(policyName, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = window,
                        QueueLimit = 0
                    }));
        }
    }
}
