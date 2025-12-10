using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    internal class InvalidTrainerProfileException : BaseDomainException
    {
        public InvalidTrainerProfileException()
        {
        }

        public InvalidTrainerProfileException(string message)
            : base(message)
        {
        }
    }
}
