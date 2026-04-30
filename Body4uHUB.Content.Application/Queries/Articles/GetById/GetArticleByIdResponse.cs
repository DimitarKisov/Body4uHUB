using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Queries.Articles.GetById
{
    public record GetArticleByIdResponse(
        int ArticleNumber,
        string Title,
        string Content,
        Guid AuthorId,
        string Status,
        DateTime? PublishedAt,
        int ViewCount,
        DateTime CreatedAt,
        DateTime? ModifiedAt,
        ICollection<CommentDto> Comments
    );
}
