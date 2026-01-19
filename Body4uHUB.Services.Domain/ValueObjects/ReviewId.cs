using Body4uHUB.Shared.Domain.Base;
using Body4uHUB.Shared.Domain.Exceptions;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ReviewConstants;

namespace Body4uHUB.Services.Domain.ValueObjects
{
    public class ReviewId : ValueObject
    {
        public int Value { get; private set; }

        private ReviewId(int value)
        {
            Value = value;
        }

        // Public - за application layer
        public static ReviewId Create(int value)
        {
            if (value <= 0)
            {
                throw new InvalidValueObjectException(ReviewIdCannotBeZeroOrNegative);
            }

            return new ReviewId(value);
        }

        // Internal - САМО за EF Core
        internal static ReviewId CreateInternal(int value)
        {
            return new ReviewId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
