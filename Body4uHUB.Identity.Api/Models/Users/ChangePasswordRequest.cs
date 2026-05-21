namespace Body4uHUB.Identity.Api.Models.Users
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
