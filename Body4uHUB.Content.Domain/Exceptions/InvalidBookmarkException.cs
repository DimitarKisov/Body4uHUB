using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions;

internal sealed class InvalidBookmarkException : BaseDomainException
{
    public InvalidBookmarkException()
    {
    }

    public InvalidBookmarkException(string message)
        : base(message)
    {
    }
}
