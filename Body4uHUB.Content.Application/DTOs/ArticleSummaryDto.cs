namespace Body4uHUB.Content.Application.DTOs;

public record ArticleSummaryDto(
    int Id,
    string Title,
    string Category,
    int ReadingMinutes,
    Guid AuthorId,
    string Status,
    DateTime? PublishedAt,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
