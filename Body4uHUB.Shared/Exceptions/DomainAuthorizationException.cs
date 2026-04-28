using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Shared.Domain.Exceptions
{
    public class DomainAuthorizationException : BaseDomainException
    {
        public DomainAuthorizationException()
        {
        }

        public DomainAuthorizationException(string message)
            : base(message)
        {
        }
    }
}
