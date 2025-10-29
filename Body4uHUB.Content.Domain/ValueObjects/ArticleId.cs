using Body4uHUB.Shared;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class ArticleId : ValueObject
    {
        public int Value { get; private set; }

        private ArticleId(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Article ID must be greater than 0");
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
