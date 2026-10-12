using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Identity.Infrastructure.Settings
{
    public sealed class PasswordHashingSettings
    {
        public const string SectionName = "Security:Hashing";

        // BCrypt cost: below 10 is too weak (OWASP), 31 is the BCrypt maximum
        [Range(10, 31)]
        public int WorkFactor { get; init; } = 12;
    }
}
