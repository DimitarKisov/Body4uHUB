using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Body4uHUB.Shared.Api.Swagger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Shared.Api.Middleware
{
    public sealed class SwaggerBasicAuthMiddleware
    {
        private const string Realm = "Swagger";

        private readonly RequestDelegate next;
        private readonly IOptionsMonitor<SwaggerOptions> optionsMonitor;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IOptionsMonitor<SwaggerOptions> optionsMonitor)
        {
            this.next = next;
            this.optionsMonitor = optionsMonitor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var options = optionsMonitor.CurrentValue;

            if (!options.RequireAuthentication)
            {
                await next(context);
                return;
            }

            if (TryAuthenticate(context, options))
            {
                await next(context);
                return;
            }

            context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\", charset=\"UTF-8\"";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        private static bool TryAuthenticate(HttpContext context, SwaggerOptions options)
        {
            if (string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password))
            {
                return false;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(header) || !AuthenticationHeaderValue.TryParse(header, out var parsed))
            {
                return false;
            }

            if (!string.Equals(parsed.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(parsed.Parameter))
            {
                return false;
            }

            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(parsed.Parameter));
            }
            catch (FormatException)
            {
                return false;
            }

            var separatorIndex = decoded.IndexOf(':');
            if (separatorIndex < 0)
            {
                return false;
            }

            var providedUser = decoded.Substring(0, separatorIndex);
            var providedPassword = decoded.Substring(separatorIndex + 1);

            return FixedTimeEquals(providedUser, options.Username)
                && FixedTimeEquals(providedPassword, options.Password);
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var aBytes = Encoding.UTF8.GetBytes(a);
            var bBytes = Encoding.UTF8.GetBytes(b);

            if (aBytes.Length != bBytes.Length)
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
        }
    }
}
