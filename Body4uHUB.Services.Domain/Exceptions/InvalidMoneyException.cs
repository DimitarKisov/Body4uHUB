using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Services.Domain.Exceptions
{
    internal class InvalidMoneyException : BaseDomainException
    {
        public InvalidMoneyException()
        {
        }

        public InvalidMoneyException(string message)
            : base(message)
        {
        }
    }
}
