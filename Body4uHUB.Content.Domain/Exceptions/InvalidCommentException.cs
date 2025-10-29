using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions
{
    internal class InvalidCommentException : BaseDomainException
    {
        public InvalidCommentException()
        {
        }

        public InvalidCommentException(string error)
            : base(error)
        {
        }
    }
}
