namespace Body4uHUB.Content.Application.DTOs
{
    public record ForumPostDto(
        int Id,
        string Content,
        Guid AuthorId,
        int TopicId,
        bool IsDeleted,
        DateTime CreatedAt,
        DateTime? ModifiedAt);
}
