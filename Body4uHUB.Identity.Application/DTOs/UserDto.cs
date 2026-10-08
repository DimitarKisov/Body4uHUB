namespace Body4uHUB.Identity.Application.DTOs
{
    public record UserDto(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        DateTime CreatedAt,
        bool IsEmailConfirmed,
        ICollection<RoleDto> Roles);
}
