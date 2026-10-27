using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Identity.Domain.Exceptions;

public class InvalidRefreshTokenException : BaseDomainException
{
    public InvalidRefreshTokenException()
    {
    }

    public InvalidRefreshTokenException(string message)
        : base(message)
    {
    }
}
