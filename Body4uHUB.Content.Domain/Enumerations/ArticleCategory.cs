using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Content.Domain.Enumerations;

public class ArticleCategory : Enumeration
{
    public static readonly ArticleCategory Training = new(1, nameof(Training));
    public static readonly ArticleCategory Nutrition = new(2, nameof(Nutrition));
    public static readonly ArticleCategory Recovery = new(3, nameof(Recovery));
    public static readonly ArticleCategory Other = new(4, nameof(Other));

    private ArticleCategory(int id, string name)
        : base(id, name)
    {
    }
}
