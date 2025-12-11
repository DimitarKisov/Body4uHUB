using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    internal class InvalidServiceOrderException : BaseDomainException
    {
        public InvalidServiceOrderException()
        {
        }

        public InvalidServiceOrderException(string message)
            : base(message)
        {
        }
    }
}
