using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Guards;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;

namespace Body4uHUB.Services.Domain.Models
{
    public class Review : Entity<ReviewId>
    {
        public Guid ClientId { get; private set; }
        public ServiceOrderId OrderId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }

        private Review()
            : base(default!)
        {
        }

        private Review(Guid reviewerId, ServiceOrderId orderId, int rating, string comment)
            : base(default!)
        {
            ClientId = reviewerId;
            OrderId = orderId;
            Rating = rating;
            Comment = comment;
        }

        internal static Review Create(Guid reviewerId, ServiceOrderId orderId, int rating, string comment)
        {
            Validate(reviewerId, orderId, rating, comment);
            return new Review(reviewerId, orderId, rating, comment);
        }

        internal void Update(int rating, string comment)
        {
            ValidateRating(rating);
            ValidateComment(comment);
            Rating = rating;
            Comment = comment;
        }

        private static void Validate(Guid clientId, ServiceOrderId orderId, int rating, string comment)
        {
            ValidateClientId(clientId);
            ValidateOrderId(orderId);
            ValidateRating(rating);
            ValidateComment(comment);
        }

        private static void ValidateClientId(Guid clientId)
        {
           Guard.AgainstEmptyGuid<InvalidReviewException>(clientId, nameof(ClientId));
        }

        private static void ValidateOrderId(ServiceOrderId orderId)
        {
            Guard.AgainstNegativeAndZero<InvalidReviewException>(orderId.Value, nameof(OrderId));
        }

        private static void ValidateRating(int rating)
        {
            Guard.AgainstOutOfRange<InvalidReviewException>(rating, MinRating, MaxRating, nameof(Rating));
        }

        private static void ValidateComment(string comment)
        {
            Guard.AgainstEmptyString<InvalidReviewException>(comment, nameof(Comment));
            Guard.ForStringLength<InvalidReviewException>(comment, MinCommentLength, MaxCommentLength, nameof(Comment));
        }
    }
}
