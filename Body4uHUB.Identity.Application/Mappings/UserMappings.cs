using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Domain.Models;

namespace Body4uHUB.Identity.Application.Mappings
{
    internal static class UserMappings
    {
        // User keeps only role Ids, so the caller loads the Role aggregates for their names.
        public static UserDto ToDto(this User user, IEnumerable<Role> roles)
            => new(
                user.Id,
                user.ContactInfo.Email,
                user.FirstName,
                user.LastName,
                user.ContactInfo.PhoneNumber,
                user.CreatedAt,
                user.IsEmailConfirmed,
                [.. roles.Select(r => r.ToDto())]);

        public static RoleDto ToDto(this Role role)
            => new(role.Id, role.Name);
    }
}
