using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions
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
