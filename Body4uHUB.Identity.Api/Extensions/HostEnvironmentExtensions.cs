namespace Body4uHUB.Identity.Api.Extensions
{
    public static class HostEnvironmentExtensions
    {
        private const string LocalEnvironmentName = "Local";

        /// <summary>
        /// True for Development and Local environments
        /// </summary>
        public static bool IsLocalLike(this IHostEnvironment environment)
        {
            return environment.IsDevelopment() || environment.IsEnvironment(LocalEnvironmentName);
        }
    }
}
