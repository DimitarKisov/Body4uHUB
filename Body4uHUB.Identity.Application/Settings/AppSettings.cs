using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Identity.Application.Settings
{
    public class AppSettings
    {
        public const string SectionName = "App";

        [Required]
        [Url]
        public string FrontendUrl { get; init; }
    }
}
