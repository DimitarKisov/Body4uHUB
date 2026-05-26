namespace Body4uHUB.Services.Api.Models.ServiceOfferings
{
    public record UpdateServiceOfferingRequest(
        string Name,
        string Description,
        decimal Price,
        string Currency,
        int DurationMinutes,
        string ServiceType);
}
