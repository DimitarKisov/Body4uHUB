using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Identity.Infrastructure.Settings
{
    public class SeedDataSettings
    {
        public const string SectionName = "SeedData";

        [Required]
        [EmailAddress]
        public string AdminUserEmail { get; init; }

        [Required]
        public string AdminUserPassword { get; init; }

        [Required]
        public string AdminUserFirstName { get; init; }

        [Required]
        public string AdminUserLastName { get; init; }

        [Required]
        public string AdminRoleName { get; init; }

        [Required]
        public List<string> UserRoles { get; init; }
    }
}
