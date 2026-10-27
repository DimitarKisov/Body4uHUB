namespace Body4uHUB.Identity.Application.DTOs
{
    /// <param name="SessionExpiresAt">When the sign-in session ends at the latest (UTC), however active the user is.</param>
    public record AuthResponseDto(
        string AccessToken,
        UserDto User,
        DateTime SessionExpiresAt);
}
