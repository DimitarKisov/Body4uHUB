using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Shared.Infrastructure.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        // HMAC-SHA256 needs a key of at least 256 bits
        [Required]
        [MinLength(32)]
        [NoPipelineToken]
        public string Secret { get; init; }

        [Required]
        public string Issuer { get; init; }

        [Required]
        public string Audience { get; init; }

        [Range(1, 1440)]
        public int ExpirationMinutes { get; init; } = 60;
    }
}
