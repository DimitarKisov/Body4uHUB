using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Domain.Models;

namespace Body4uHUB.Identity.Application.Mappings
{
    internal static class UserMappings
    {
        public static UserDto ToDto(this User user)
            => new(
                user.Id,
                user.ContactInfo.Email,
                user.FirstName,
                user.LastName,
                user.ContactInfo.PhoneNumber,
                user.CreatedAt,
                user.IsEmailConfirmed,
                [.. user.Roles.Select(r => r.ToDto())]);

        public static RoleDto ToDto(this Role role)
            => new(role.Id, role.Name);
    }
}
