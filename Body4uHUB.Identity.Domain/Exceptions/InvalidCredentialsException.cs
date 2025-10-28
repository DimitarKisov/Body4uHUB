using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Identity.Domain.Exceptions
{
    public class InvalidCredentialsException : BaseDomainException
    {
        public InvalidCredentialsException()
        {
        }

        public InvalidCredentialsException(string message)
            : base(message)
        {
        }
    }
}
