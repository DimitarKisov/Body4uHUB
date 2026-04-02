namespace Body4uHUB.Shared.Application
{
    public record AuthorizationContext(Guid CurrentUserId, bool IsAdmin)
    {
        public static AuthorizationContext Create(Guid userId, bool isAdmin)
        {
            return new AuthorizationContext(userId, isAdmin);
        }
    }
}
