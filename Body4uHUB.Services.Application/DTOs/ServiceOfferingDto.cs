namespace Body4uHUB.Services.Application.DTOs
{
    public class ServiceOfferingDto
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int DurationMinutes { get; set; }
        public string ServiceType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
