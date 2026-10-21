using System.Collections.Concurrent;
using System.Net;

namespace Body4uHUB.Shared.Infrastructure.Email
{
    /// <summary>
    /// Branded HTML emails embedded from Email/Templates (copied from design/emails).
    /// Placeholders are {{name}}; values are HTML-encoded, so a user's name can't inject markup.
    /// </summary>
    internal static class EmailTemplates
    {
        public const string ConfirmEmail = "confirm-email";
        public const string ResetPassword = "reset-password";

        private static readonly ConcurrentDictionary<string, string> Cache = new();

        public static string Render(string name, IReadOnlyDictionary<string, string> values)
        {
            var html = Cache.GetOrAdd(name, Load);
            foreach (var (key, value) in values)
            {
                html = html.Replace("{{" + key + "}}", WebUtility.HtmlEncode(value));
            }

            return html;
        }

        private static string Load(string name)
        {
            var resource = $"EmailTemplates.{name}.html";
            using var stream = typeof(EmailTemplates).Assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"Email template '{resource}' is not embedded.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
