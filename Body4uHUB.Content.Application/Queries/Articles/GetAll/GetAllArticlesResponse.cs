namespace Body4uHUB.Content.Application.Queries.Articles.GetAll
{
    public record GetAllArticlesResponse(
        int Id,
        string Title,
        Guid AuthorId,
        string Status,
        DateTime? PublishedAt,
        int ViewCount,
        DateTime CreatedAt,
        DateTime? ModifiedAt
    );
}
