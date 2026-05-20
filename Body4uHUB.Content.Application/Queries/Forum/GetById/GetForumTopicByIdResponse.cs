using Body4uHUB.Content.Application.DTOs;

namespace Body4uHUB.Content.Application.Queries.Forum.GetById
{
    public record GetForumTopicByIdResponse(
        int Id,
        string Title,
        Guid AuthorId,
        bool IsLocked,
        int ViewCount,
        int PostCount,
        DateTime CreatedAt,
        DateTime? ModifiedAt,
        ICollection<ForumPostDto> Posts);
}
