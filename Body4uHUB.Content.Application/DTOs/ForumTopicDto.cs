namespace Body4uHUB.Content.Application.DTOs;

public record ForumTopicDto(
    int Id,
    string Title,
    Guid AuthorId,
    bool IsLocked,
    int ViewCount,
    int PostCount,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
