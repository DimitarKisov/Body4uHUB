namespace Body4uHUB.Identity.Application.Queries.GetPublicUsers
{
    // Only what other services' pages show next to content; no contact details or roles.
    public record PublicUserResponse(
        Guid Id,
        string FirstName,
        string LastName);
}
