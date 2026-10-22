using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Content.Domain.Exceptions;

internal sealed class InvalidArticleException : BaseDomainException
{
    public InvalidArticleException()
    {
    }

    public InvalidArticleException(string message)
        : base(message)
    {
    }
}
