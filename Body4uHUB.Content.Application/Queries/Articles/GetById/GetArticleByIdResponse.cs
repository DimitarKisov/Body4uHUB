using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Queries.Articles.GetById;

public record GetArticleByIdResponse(
    int Id,
    string Title,
    string Content,
    string Category,
    int ReadingMinutes,
    Guid AuthorId,
    string Status,
    DateTime? PublishedAt,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    bool IsBookmarked,
    ICollection<CommentDto> Comments
);
