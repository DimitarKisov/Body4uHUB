namespace Body4uHUB.Shared.Application
{
    /// <summary>
    /// Error types for different HTTP status codes
    /// </summary>
    public enum ErrorType
    {
        None,
        Unauthorized,           // 401 Unauthorized - невалидна автентикация (invalid credentials)
        Forbidden,              // 403 Forbidden - валидна автентикация, но няма права
        ResourceNotFound,       // 422 Unprocessable Entity - ресурс не съществува в базата
        Conflict,               // 409 Conflict - конфликт с existing data (email already exists)
        BusinessRule            // 422 Unprocessable Entity - нарушено бизнес правило (domain logic)
    }

    /// <summary>
    /// Represents the result of an operation without a return value
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public ErrorType ErrorType { get; }

        protected Result(bool isSuccess, string error, ErrorType errorType = ErrorType.None)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("Success result cannot have an error");
            }
            if (!isSuccess && string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException("Failure result must have an error");
            }

            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        // Success
        public static Result Success() => new Result(true, string.Empty);

        // Failures
        public static Result Unauthorized(string error)
            => new Result(false, error, ErrorType.Unauthorized);

        public static Result Forbidden(string error)
            => new Result(false, error, ErrorType.Forbidden);

        public static Result ResourceNotFound(string error)
            => new Result(false, error, ErrorType.ResourceNotFound);

        public static Result Conflict(string error)
            => new Result(false, error, ErrorType.Conflict);

        public static Result BusinessRuleViolation(string error)
            => new Result(false, error, ErrorType.BusinessRule);

        // Generic methods
        public static Result<T> Success<T>(T value)
            => new Result<T>(value, true, string.Empty, ErrorType.None);

        public static Result<T> Unauthorized<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Unauthorized);

        public static Result<T> Forbidden<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Forbidden);

        public static Result<T> ResourceNotFound<T>(string error)
            => new Result<T>(default, false, error, ErrorType.ResourceNotFound);

        public static Result<T> Conflict<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Conflict);

        public static Result<T> BusinessRuleViolation<T>(string error)
            => new Result<T>(default, false, error, ErrorType.BusinessRule);
    }

    /// <summary>
    /// Represents the result of an operation with a return value
    /// </summary>
    public class Result<T> : Result
    {
        public T Value { get; }

        protected internal Result(T value, bool isSuccess, string error, ErrorType errorType = ErrorType.None)
            : base(isSuccess, error, errorType)
        {
            Value = value;
        }
    }
}