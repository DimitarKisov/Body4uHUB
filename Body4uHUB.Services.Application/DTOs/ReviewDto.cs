using Body4uHUB.Services.Domain.ValueObjects;

namespace Body4uHUB.Services.Application.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public ServiceOrderId ServiceOrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
