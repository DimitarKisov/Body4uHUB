using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    public class InvalidTrainerProfileException : BaseDomainException
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
