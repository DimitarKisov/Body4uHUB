namespace Body4uHUB.Identity.Domain.Exceptions
{
    using Body4uHUB.Shared.Exceptions;

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
}
