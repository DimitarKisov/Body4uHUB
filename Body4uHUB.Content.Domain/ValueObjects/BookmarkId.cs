using Body4uHUB.Shared;

using static Body4uHUB.Content.Domain.Constants.ModelConstants.Bookmark;

namespace Body4uHUB.Content.Domain.ValueObjects
{
    public class BookmarkId : ValueObject
    {
        public int Value { get; private set; }

        private BookmarkId(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException(BookmarkIdCannotBeZeroOrNegative);
            }

            Value = value;
        }

        public static BookmarkId Create(int value)
        {
            return new BookmarkId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
