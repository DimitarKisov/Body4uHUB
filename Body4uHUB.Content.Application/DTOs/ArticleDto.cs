namespace Body4uHUB.Content.Application.DTOs
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public Guid AuthorId { get; set; }
        public string Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
