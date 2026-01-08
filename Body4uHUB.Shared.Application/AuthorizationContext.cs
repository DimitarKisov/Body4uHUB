namespace Body4uHUB.Shared.Application
{
    public class AuthorizationContext
    {
        public Guid CurrentUserId { get; init; }
        public bool IsAdmin { get; init; }

        public static AuthorizationContext Create(Guid userId, bool isAdmin)
        {
            return new AuthorizationContext
            {
                CurrentUserId = userId,
                IsAdmin = isAdmin
            };
        }
    }
}
