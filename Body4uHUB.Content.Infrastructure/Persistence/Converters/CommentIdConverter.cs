using Body4uHUB.Content.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Content.Infrastructure.Persistence.Converters
{
    public class CommentIdConverter : ValueConverter<CommentId, int>
    {
        public CommentIdConverter()
            : base(
                commentId => commentId.Value,
                value => CommentId.Create(value))
        {
        }
    }
}
