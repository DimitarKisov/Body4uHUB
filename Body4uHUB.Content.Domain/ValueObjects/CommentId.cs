using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class CommentId : ValueObject
    {
        public int Value { get; private set; }

        private CommentId(int value)
        {
            Value = value;
        }

        // Public - за application layer
        public static CommentId Create(int value)
        {
            if (value <= 0)
            {
                throw new InvalidValueObjectException(CommentIdCannotBeZeroOrNegative);
            }

            return new CommentId(value);
        }

        // Internal - САМО за EF Core
        internal static CommentId CreateInternal(int value)
        {
            return new CommentId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
