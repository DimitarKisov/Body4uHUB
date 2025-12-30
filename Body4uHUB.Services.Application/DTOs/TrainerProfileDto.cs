namespace Body4uHUB.Services.Application.DTOs
{
    public class TrainerProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Bio { get; set; }
        public List<string> Specializations { get; set; }
        public List<string> Certifications { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<ServiceOfferingDto> ServiceOfferings { get; set; }
    }
}
