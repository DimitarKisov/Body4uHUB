using Body4uHUB.Shared.Exceptions;

namespace Body4uHUB.Shared
{
    public static class Guard
    {
        public static void AgainstDefault<TException, T>(T value, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (!EqualityComparer<T>.Default.Equals(value, default!))
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be the default value.");
        }

        public static void AgainstEmptyGuid<TException>(Guid value, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (value != Guid.Empty)
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be an empty GUID.");
        }

        public static void AgainstEmptyString<TException>(string value, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (!string.IsNullOrEmpty(value))
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be null or empty.");
        }

        public static void AgainstNegative<TException>(int value, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (value >= 0)
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be negative.");
        }

        public static void AgainstOutOfRange<TException>(int value, int minValue, int maxValue, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (value >= minValue && value <= maxValue)
            {
                return;
            }

            ThrowException<TException>($"{name} must be between {minValue} and {maxValue}.");
        }

        public static void AgainstOutOfRange<TException>(decimal value, decimal minValue, decimal maxValue, string name = "Value")
            where TException : BaseDomainException, new()
        {
            if (value >= minValue && value <= maxValue)
            {
                return;
            }

            ThrowException<TException>($"{name} must be between {minValue} and {maxValue}.");
        }

        public static void AgainstNotContainingSpecialChars<TException>(string value, string message, string specialChars = "!@#$%^&*()")
            where TException : BaseDomainException, new()
        {
            if (value != null && value.Any(specialChars.Contains))
            {
                return;
            }

            ThrowException<TException>(message);
        }

        public static void AgainstPastDate<TException>(DateTime date, string name = "Date")
            where TException : BaseDomainException, new()
        {
            if (date >= DateTime.UtcNow)
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be in the past.");
        }

        public static void AgainstMoreThanOneYearInFuture<TException>(DateTime date, string name = "Date")
            where TException : BaseDomainException, new()
        {
            if (date <= DateTime.UtcNow.AddYears(1))
            {
                return;
            }

            ThrowException<TException>($"{name} cannot be more than one year in the future.");
        }

        public static void AgainstWrongMoneyCurrency<TException>(string currency, string name = "Currency")
            where TException : BaseDomainException, new()
        {
            var expectedCurrecnies = new HashSet<string> { "BGN", "EUR" };
            if (expectedCurrecnies.Contains(currency.ToUpper()))
            {
                return;
            }

            ThrowException<TException>($"{currency} is not a supported currency.");
        }

        public static void ForStringLength<TException>(string value, int minLength, int maxLength, string name = "Value")
            where TException : BaseDomainException, new()
        {
            AgainstEmptyString<TException>(value, name);

            if (minLength <= value.Length && value.Length <= maxLength)
            {
                return;
            }

            ThrowException<TException>($"{name} must have between {minLength} and {maxLength} symbols.");
        }

        private static void ThrowException<TException>(string message)
            where TException : BaseDomainException, new()
        {
            var exception = new TException
            {
                Error = message
            };

            throw exception;
        }
    }
}
