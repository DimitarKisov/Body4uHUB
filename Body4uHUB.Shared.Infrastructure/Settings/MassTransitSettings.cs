using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Shared.Infrastructure.Settings
{
    public class MassTransitSettings
    {
        public const string SectionName = "MassTransit";

        [Required]
        [NoPipelineToken]
        public string Host { get; init; }

        [Range(1, 65535)]
        public int Port { get; init; } = 5672;

        [Required]
        public string VirtualHost { get; init; } = "/";

        [Required]
        [NoPipelineToken]
        public string Username { get; init; }

        [Required]
        [NoPipelineToken]
        public string Password { get; init; }

        [Range(0, 20)]
        public int RetryCount { get; init; } = 5;

        [Range(1, 300)]
        public int RetryIntervalSeconds { get; init; } = 5;

        [Range(1, 1000)]
        public int OutboxQueryMessageLimit { get; init; } = 100;

        [Range(1, 60)]
        public int OutboxQueryDelaySeconds { get; init; } = 1;
    }
}
