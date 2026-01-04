using Body4uHUB.Services.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Services.Infrastructure.Persistence.Converters
{
    public class ServiceOfferingConverter : ValueConverter<ServiceOfferingId, int>
    {
        public ServiceOfferingConverter()
            : base(
                serviceOfferingId => serviceOfferingId.Value,
                value => ServiceOfferingId.CreateInternal(value))
        {
        }
    }
}
