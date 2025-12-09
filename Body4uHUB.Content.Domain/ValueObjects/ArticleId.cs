using Body4uHUB.Content.Domain.Exceptions;
using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.ArticleConstants;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class ArticleId : ValueObject
    {
        public int Value { get; private set; }

        private ArticleId(int value)
        {
            Value = value;
        }

        // Public - за application layer
        public static ArticleId Create(int value)
        {
            if (value <= 0)
            {
                throw new InvalidValueObjectException(IdCannotBeZeroOrNegative);
            }

            return new ArticleId(value);
        }

        // Internal - САМО за EF Core
        internal static ArticleId CreateInternal(int value)
        {
            return new ArticleId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
