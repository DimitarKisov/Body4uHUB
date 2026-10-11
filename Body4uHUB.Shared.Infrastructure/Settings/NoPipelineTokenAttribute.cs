using System.ComponentModel.DataAnnotations;

namespace Body4uHUB.Shared.Infrastructure.Settings
{
    /// <summary>
    /// The deploy pipeline replaces '#{Name}#' tokens in appsettings.json.
    /// A value that is still a token means the pipeline variable is missing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NoPipelineTokenAttribute : ValidationAttribute
    {
        public NoPipelineTokenAttribute()
            : base("The {0} field is an unreplaced '#{{...}}#' token. The deploy pipeline variable is missing.")
        {
        }

        public override bool IsValid(object value)
        {
            return value is not string text || !(text.StartsWith("#{") && text.EndsWith("}#"));
        }
    }
}
