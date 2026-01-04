using Body4uHUB.Services.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Body4uHUB.Services.Infrastructure.Persistence.Converters
{
    public class ServiceOrderConverter : ValueConverter<ServiceOrderId, int>
    {
        public ServiceOrderConverter()
            : base(
                serviceOrderId => serviceOrderId.Value,
                value => ServiceOrderId.CreateInternal(value))
        { 
        }
    }
}
