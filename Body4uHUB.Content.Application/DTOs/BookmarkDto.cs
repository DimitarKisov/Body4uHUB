namespace Body4uHUB.Content.Application.DTOs
{
    public class BookmarkDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ArticleDto Article { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}