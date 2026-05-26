namespace Body4uHUB.Services.Application.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public int ServiceOrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
