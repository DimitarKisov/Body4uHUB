namespace Body4uHUB.Services.Api.Models.ServiceOfferings
{
    public record AddServiceOfferingRequest(
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int DurationMinutes,
        string ServiceType,
        int MaxParticipants,
        bool IsOnline,
        DateTime? StartDate,
        DateTime? EndDate);
}
