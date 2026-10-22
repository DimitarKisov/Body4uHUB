namespace Body4uHUB.Content.Application.DTOs
{
    public record CommentDto(
        Guid Id,
        string Content,
        Guid AuthorId,
        Guid? ParentCommentId,
        bool IsDeleted,
        DateTime CreatedAt,
        DateTime? ModifiedAt);
}
