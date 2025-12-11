using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    internal class InvalidServiceOfferingException : BaseDomainException
    {
        public InvalidServiceOfferingException()
        {
        }

        public InvalidServiceOfferingException(string message)
            : base(message)
        {
        }
    }
}
