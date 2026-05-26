namespace Body4uHUB.Services.Application.DTOs
{
    public class ServiceOfferingDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int? DurationInMinutes { get; set; }
        public string ServiceCategory { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
