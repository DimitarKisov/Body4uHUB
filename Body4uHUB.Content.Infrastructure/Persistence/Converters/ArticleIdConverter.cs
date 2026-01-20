using Body4uHUB.Content.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Content.Infrastructure.Persistence.Converters
{
    //THOSE THINGS ARE NOT USED ANYMORE. IN THIS PROJECT WE USE VALUE OBJECTS WITHOUT CONVERTERS. KEEPING THIS CODE FOR FUTURE CHANGES IF NEEDED.
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