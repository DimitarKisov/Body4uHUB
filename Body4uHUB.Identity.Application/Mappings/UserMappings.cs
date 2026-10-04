using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Domain.Models;

namespace Body4uHUB.Identity.Application.Mappings
{
    internal static class UserMappings
    {
        public static UserDto ToDto(this User user)
            => new()
            {
                Id = user.Id,
                Email = user.ContactInfo.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.ContactInfo.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsEmailConfirmed = user.IsEmailConfirmed,
                Roles = [.. user.Roles.Select(r => r.ToDto())]
            };

        public static RoleDto ToDto(this Role role)
            => new()
            {
                Id = role.Id,
                Name = role.Name
            };
    }
}
