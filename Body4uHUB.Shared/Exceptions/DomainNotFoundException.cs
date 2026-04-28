using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Shared.Domain.Exceptions
{
    public class DomainNotFoundException : BaseDomainException
    {
        public DomainNotFoundException()
        {
        }

        public DomainNotFoundException(string message)
            : base(message)
        {
        }
    }
}
