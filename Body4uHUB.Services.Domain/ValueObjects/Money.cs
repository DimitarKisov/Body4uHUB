using Body4uHUB.Services.Domain.Exceptions;
using Body4uHUB.Shared;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency)
        {
            Validate(amount, currency);
            return new Money(amount, currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        private static void Validate(decimal amount, string currency)
        {
            Guard.AgainstOutOfRange<InvalidMoneyException>(amount, MinMoneyAmount, MaxMoneyAmount, nameof(Amount));
            Guard.AgainstWrongMoneyCurrency<InvalidMoneyException>(currency, nameof(Currency));
        }
    }
}
