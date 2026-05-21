namespace Body4uHUB.Shared.Api.Swagger
{
    public sealed class SwaggerOptions
    {
        public const string SectionName = "Swagger";

        public bool Enabled { get; set; }

        public bool RequireAuthentication { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
