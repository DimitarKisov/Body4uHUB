namespace Body4uHUB.Services.Api.Models.Reviews
{
    public record AddReviewRequest(int OrderId, int Rating, string Comment);
}
