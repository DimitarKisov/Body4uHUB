using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Identity.Application.Settings
{
    public class RefreshTokenSettings
    {
        public const string SectionName = "RefreshToken";

        /// <summary>A session ends after this many days without a refresh.</summary>
        [Range(1, 90)]
        public int InactivityDays { get; init; } = 7;

        /// <summary>A session ends this many days after sign-in, however active it is.</summary>
        [Range(1, 365)]
        public int LifetimeDays { get; init; } = 30;

        public TimeSpan Inactivity => TimeSpan.FromDays(InactivityDays);
        public TimeSpan Lifetime => TimeSpan.FromDays(Math.Max(LifetimeDays, InactivityDays));
    }
}
