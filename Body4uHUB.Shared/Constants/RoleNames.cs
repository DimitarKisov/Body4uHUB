namespace Body4uHUB.Shared.Domain.Constants
{
    // Identity seeds these roles and puts them in the JWT; Content and Services authorize by them.
    public static class RoleNames
    {
        public const string Administrator = "Administrator";
        public const string Trainer = "Trainer";
        public const string User = "User";

        public static readonly IReadOnlyList<string> All = [Administrator, Trainer, User];
    }
}
