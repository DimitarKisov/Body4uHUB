using Body4uHUB.Shared.Infrastructure.Settings;
using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Content.Infrastructure.Settings
{
    public class ViewTrackingSettings
    {
        public const string SectionName = "ViewTracking";

        /// <summary>
        /// Secret key for hashing visitor IPs, so raw IP addresses are never stored.
        /// </summary>
        [Required]
        [MinLength(32)]
        [NoPipelineToken]
        public string HashKey { get; init; }
    }
}
