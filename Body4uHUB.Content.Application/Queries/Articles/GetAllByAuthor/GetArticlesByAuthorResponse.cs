namespace Body4uHUB.Content.Application.Queries.Articles.GetAllByAuthor
{
    public record GetArticlesByAuthorResponse(
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
