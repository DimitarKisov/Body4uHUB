using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.CommentConstants;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class CommentId : ValueObject
    {
        public int Value { get; private set; }

        private CommentId(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException(CommentIdCannotBeZeroOrNegative);
            }

            Value = value;
        }

        public static CommentId Create(int value)
        {
            return new CommentId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
