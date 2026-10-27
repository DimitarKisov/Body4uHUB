namespace Body4uHUB.Content.Application.DTOs;

public record ForumTopicDto(
    int Id,
    string Title,
    Guid AuthorId,
    bool IsLocked,
    bool IsDeleted, // only an admin listing with includeDeleted=true has true here
    int ViewCount,
    int PostCount,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
