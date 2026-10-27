namespace Body4uHUB.Identity.Application.DTOs
{
    /// <summary>
    /// What login and refresh hand to the API layer: the JSON body plus the refresh token, which goes into an
    /// HttpOnly cookie and never into the body.
    /// </summary>
    /// <param name="IsPersistent">"Запомни ме": the cookie outlives the browser session (until <paramref name="RefreshTokenExpiresAt"/>).</param>
    public record AuthSession(
        AuthResponseDto Response,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt,
        bool IsPersistent);
}
