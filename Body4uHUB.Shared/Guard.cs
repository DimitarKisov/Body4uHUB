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

        public static void AgainstNotContainingSpecialChars<TException>(string value, string message, string specialChars = "!@#$%^&*()")
            where TException : BaseDomainException, new()
        {
            if (value != null && value.Any(specialChars.Contains))
            {
                return;
            }

            ThrowException<TException>(message);
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
