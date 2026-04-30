namespace Body4uHUB.Shared.Application
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int TotalCount { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNext => Page < TotalPages;
        public bool HasPrevious => Page > 1;

        public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        {
            if (page < 1)
            {
                throw new ArgumentException("Page must be >= 1", nameof(page));
            }
            if (pageSize < 1)
            {
                throw new ArgumentException("PageSize must be >= 1", nameof(pageSize));
            }
            if (totalCount < 0)
            {
                throw new ArgumentException("TotalCount cannot be negative", nameof(totalCount));
            }

            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }

        public static PagedResult<T> Empty(int page, int pageSize)
            => new(Array.Empty<T>(), 0, page, pageSize);
    }
}
