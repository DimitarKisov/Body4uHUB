using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Content.Domain.Enumerations
{
    public class ArticleStatus : Enumeration
    {
        public static readonly ArticleStatus Draft = new(1, nameof(Draft));
        public static readonly ArticleStatus Published = new(2, nameof(Published));
        public static readonly ArticleStatus Archived = new(3, nameof(Archived));

        private ArticleStatus(int id, string name)
            : base(id, name)
        {
        }
    }
}
