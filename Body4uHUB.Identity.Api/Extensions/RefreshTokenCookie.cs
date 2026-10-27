using Body4uHUB.Identity.Application.DTOs;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.RefreshTokenConstants;

namespace Body4uHUB.Identity.Api.Extensions
{
    /// <summary>
    /// The refresh token lives only in this cookie: HttpOnly (scripts cannot read it), Secure, SameSite=Strict
    /// (not sent from other sites) and scoped to /api. Only refresh, logout and change-password read it; every other
    /// endpoint authenticates with the Bearer token alone, so the cookie cannot be used for cross-site requests.
    /// </summary>
    public static class RefreshTokenCookie
    {
        public static string Read(HttpRequest request) => request.Cookies[CookieName];

        public static void Write(HttpResponse response, AuthSession session)
        {
            var options = BaseOptions();

            // Without "Запомни ме" it is a session cookie: the browser drops it on close.
            if (session.IsPersistent)
            {
                options.Expires = new DateTimeOffset(session.RefreshTokenExpiresAt, TimeSpan.Zero);
            }

            response.Cookies.Append(CookieName, session.RefreshToken, options);
        }

        public static void Delete(HttpResponse response) => response.Cookies.Delete(CookieName, BaseOptions());

        private static CookieOptions BaseOptions() => new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = CookiePath,
            IsEssential = true,
        };
    }
}
