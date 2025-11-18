using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Identity.Domain.Exceptions
{
    public class InvalidRoleException : BaseDomainException
    {
        public InvalidRoleException()
        {
        }

        public InvalidRoleException(string message)
            : base(message)
        {
        }
    }
}
