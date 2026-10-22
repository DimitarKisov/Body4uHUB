using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions;

internal sealed class InvalidForumPostException : BaseDomainException
{
    public InvalidForumPostException()
    {
    }

    public InvalidForumPostException(string message)
        : base(message)
    {
    }
}
