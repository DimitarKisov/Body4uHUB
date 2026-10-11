using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Shared.Infrastructure.Settings
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        [Required]
        [NoPipelineToken]
        public string SmtpServer { get; init; }

        [Range(1, 65535)]
        public int SmtpPort { get; init; } = 587;

        public bool EnableSsl { get; init; } = true;

        [Required]
        [NoPipelineToken]
        public string Username { get; init; }

        [Required]
        [NoPipelineToken]
        public string Password { get; init; }

        [Required]
        [EmailAddress]
        [NoPipelineToken]
        public string FromEmail { get; init; }

        [Required]
        public string FromName { get; init; }
    }
}
