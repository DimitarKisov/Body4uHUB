namespace Body4uHUB.Content.Application.DTOs
{
    public class ForumTopicDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public bool IsLocked { get; set; }
        public int ViewCount { get; set; }
        public int PostCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}