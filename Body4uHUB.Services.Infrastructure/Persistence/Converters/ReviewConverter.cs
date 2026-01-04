using Body4uHUB.Services.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Services.Infrastructure.Persistence.Converters
{
    public class ReviewConverter : ValueConverter<ReviewId, int>
    {
        public ReviewConverter()
            : base(
                reviewId => reviewId.Value,
                value => ReviewId.CreateInternal(value))
        {
        }
    }
}
