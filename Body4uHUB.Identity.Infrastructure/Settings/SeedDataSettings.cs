using Body4uHUB.Shared.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Identity.Infrastructure.Settings
{
    public sealed class SeedDataSettings
    {
        public const string SectionName = "SeedData";

        [Required]
        [EmailAddress]
        [NoPipelineToken]
        public string AdminUserEmail { get; init; }

        [Required]
        [NoPipelineToken]
        public string AdminUserPassword { get; init; }

        [Required]
        public string AdminUserFirstName { get; init; }

        [Required]
        public string AdminUserLastName { get; init; }
    }
}
