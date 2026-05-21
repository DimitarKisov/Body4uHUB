namespace Body4uHUB.Content.Application.Queries.Bookmarks.GetUserBookmarks
{
    public record GetUserBookmarksResponse(
        Guid Id,
        int ArticleId,
        string ArticleTitle,
        Guid ArticleAuthorId,
        DateTime CreatedAt
    );
}
