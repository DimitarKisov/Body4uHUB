using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.Article;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class ArticleId : ValueObject
    {
        public int Value { get; private set; }

        private ArticleId(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException(IdCannotBeZeroOrNegative);
            }

            Value = value;
        }

        public static ArticleId Create(int value)
        {
            return new ArticleId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
