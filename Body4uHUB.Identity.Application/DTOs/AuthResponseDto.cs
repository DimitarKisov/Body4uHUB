namespace Body4uHUB.Identity.Application.DTOs
{
    public record AuthResponseDto(
        string AccessToken,
        UserDto User);
}
