using Body4uHUB.Shared;
using Body4uHUB.Shared.Domain.Exceptions;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.ServiceOfferingConstants;

namespace Body4uHUB.Services.Domain.ValueObjects
{
    public class ServiceOfferingId : ValueObject
    {
        public int Value { get; private set; }

        private ServiceOfferingId(int value)
        {
            Value = value;
        }

        // Public - за application layer
        public static ServiceOfferingId Create(int value)
        {
            Validate(value);
            return new ServiceOfferingId(value);
        }

        // Internal - САМО за EF Core
        internal static ServiceOfferingId CreateInternal(int value)
        {
            return new ServiceOfferingId(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static void Validate(int value)
        {
            Guard.AgainstNegative<InvalidValueObjectException>(value, ServiceOfferingIdCannotBeZeroOrNegative);
        }
    }
}
