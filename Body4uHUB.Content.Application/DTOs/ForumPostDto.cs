namespace Body4uHUB.Content.Application.DTOs
{
    public class ForumPostDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public int TopicId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}