namespace Body4uHUB.Services.Api.Models.ServiceOrders
{
    public record CreateServiceOrderRequest(Guid TrainerId, int ServiceOfferingId, string Notes);
}
