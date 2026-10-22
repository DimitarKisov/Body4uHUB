using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions;

internal sealed class InvalidForumTopicException : BaseDomainException
{
    public InvalidForumTopicException()
    {
    }

    public InvalidForumTopicException(string message)
        : base(message)
    {
    }
}
