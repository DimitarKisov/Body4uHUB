using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Identity.Domain.Exceptions
{
    public class InvalidUserException : BaseDomainException
    {
        public InvalidUserException()
        {
        }

        public InvalidUserException(string message)
            : base(message)
        {
        }
    }
}
