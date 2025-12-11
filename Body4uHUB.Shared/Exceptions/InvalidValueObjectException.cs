using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Shared.Domain.Exceptions
{
    public class InvalidValueObjectException : BaseDomainException
    {
        public InvalidValueObjectException()
        {
        }

        public InvalidValueObjectException(string message)
            : base(message)
        {
        }
    }
}
