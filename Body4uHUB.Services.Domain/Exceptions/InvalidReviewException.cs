using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    internal class InvalidReviewException : BaseDomainException
    {
        public InvalidReviewException()
        {
        }

        public InvalidReviewException(string message)
            : base(message)
        {
        }
    }
}
