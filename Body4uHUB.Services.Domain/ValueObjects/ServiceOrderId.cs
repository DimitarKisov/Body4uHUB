using Body4uHUB.Shared;
using Body4uHUB.Shared.Domain.Exceptions;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOrderConstants;

namespace Body4uHUB.Services.Domain.ValueObjects
{
    public class ServiceOrderId : ValueObject
    {
        public int Value { get; private set; }

        private ServiceOrderId(int value)
        {
            Value = value;
        }

        // Public - за application layer
        public static ServiceOrderId Create(int value)
        {
            if (value <= 0)
            {
                throw new InvalidValueObjectException(ServiceOrderIdCannotBeZeroOrNegative);
            }

            return new ServiceOrderId(value);
        }

        // Internal - САМО за EF Core
        internal static ServiceOrderId CreateInternal(int value)
        {
            return new ServiceOrderId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
