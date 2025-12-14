using Body4uHUB.Services.Domain.ValueObjects;

namespace Body4uHUB.Services.Application.DTOs
{
    public class ServiceOrderDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid TrainerProfileId { get; set; }
        public ServiceOfferingId ServiceOfferingId { get; set; }
        public string ServiceName { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; }
        public ReviewDto Review { get; set; }
    }
}
