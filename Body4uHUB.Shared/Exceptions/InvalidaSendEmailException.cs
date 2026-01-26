using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Shared.Domain.Exceptions
{
    public class InvalidaSendEmailException : BaseDomainException
    {
        public InvalidaSendEmailException()
        {
        }

        public InvalidaSendEmailException(string message)
            : base(message)
        {
        }
    }
}
