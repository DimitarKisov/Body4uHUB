using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Identity.Domain.Exceptions;

public class InvalidContactInfoException : BaseDomainException
{
    public InvalidContactInfoException()
    {
    }

    public InvalidContactInfoException(string message)
        : base(message)
    {
    }
}
