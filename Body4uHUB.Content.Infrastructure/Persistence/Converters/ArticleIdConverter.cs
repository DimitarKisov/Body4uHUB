using Body4uHUB.Content.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Content.Infrastructure.Persistence.Converters
{
    public class ArticleIdConverter : ValueConverter<ArticleId, int>
    {
        public ArticleIdConverter()
            : base(
                articleId => articleId.Value,
                value => ArticleId.CreateInternal(value))
        {
        }
    }
}