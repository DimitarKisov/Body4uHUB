using Body4uHUB.Content.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Content.Infrastructure.Persistence.Converters
{
    //THOSE THINGS ARE NOT USED ANYMORE. IN THIS PROJECT WE USE VALUE OBJECTS WITHOUT CONVERTERS. KEEPING THIS CODE COMMENTED FOR FUTURE CHANGES IF NEEDED.
    public class CommentIdConverter : ValueConverter<CommentId, int>
    {
        public CommentIdConverter()
            : base(
                commentId => commentId.Value,
                value => CommentId.CreateInternal(value))
        {
        }
    }
}
