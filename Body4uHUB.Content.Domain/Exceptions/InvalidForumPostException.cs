using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions
{
    internal class InvalidForumPostException : BaseDomainException
    {
        public InvalidForumPostException()
        {
        }

        public InvalidForumPostException(string message)
            : base(message)
        {
        }
    }
}
