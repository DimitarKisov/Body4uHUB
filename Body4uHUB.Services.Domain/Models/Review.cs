using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Services.Domain.ValueObjects;
using Body4uHUB.Shared;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;

namespace Body4uHUB.Services.Domain.Models
{
    public class Review : Entity<ReviewId>
    {
        public int Rating { get; private set; }
        public string Comment { get; private set; }

        private Review()
            : base(default!)
        {
        }

        private Review(int rating, string comment)
            : base(default!)
        {
            Rating = rating;
            Comment = comment;
        }

        internal static Review Create(int rating, string comment)
        {
            Validate(rating, comment);
            return new Review(rating, comment);
        }

        internal void Update(int rating, string comment)
        {
            Validate(rating, comment);
            Rating = rating;
            Comment = comment;
        }

        private static void Validate(int rating, string comment)
        {
            ValidateRating(rating);
            ValidateComment(comment);
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
