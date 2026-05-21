namespace Body4uHUB.Identity.Application.Queries.GetAllUsers
{
    public record GetAllUsersResponse(
        Guid Id,
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        DateTime CreatedAt,
        bool IsEmailConfirmed);
}
