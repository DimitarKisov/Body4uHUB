namespace Body4uHUB.Shared.Application
{
    /// <summary>
    /// Error types for different HTTP status codes
    /// </summary>
    public enum ErrorType
    {
        None,
        Validation,             // 400 Bad Request - валидационни грешки от FluentValidation
        Conflict,               // 409 Conflict - конфликт с existing data (email already exists)
        Unauthorized,           // 401 Unauthorized - невалидна автентикация (invalid credentials)
        Forbidden,              // 403 Forbidden - валидна автентикация, но няма права (email not confirmed)
        UnprocessableEntity     // 422 Unprocessable Entity - ресурс не е намерен (user not found, article not found)
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

        public static Result Success() => new Result(true, string.Empty);

        public static Result Failure(string error, ErrorType errorType = ErrorType.Validation)
            => new Result(false, error, errorType);

        public static Result Conflict(string error)
            => new Result(false, error, ErrorType.Conflict);

        public static Result Unauthorized(string error)
            => new Result(false, error, ErrorType.Unauthorized);

        public static Result Forbidden(string error)
            => new Result(false, error, ErrorType.Forbidden);

        public static Result UnprocessableEntity(string error)
            => new Result(false, error, ErrorType.UnprocessableEntity);

        // Generic methods
        public static Result<T> Success<T>(T value)
            => new Result<T>(value, true, string.Empty, ErrorType.None);

        public static Result<T> Failure<T>(string error, ErrorType errorType = ErrorType.Validation)
            => new Result<T>(default, false, error, errorType);

        public static Result<T> Conflict<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Conflict);

        public static Result<T> Unauthorized<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Unauthorized);

        public static Result<T> Forbidden<T>(string error)
            => new Result<T>(default, false, error, ErrorType.Forbidden);

        public static Result<T> UnprocessableEntity<T>(string error)
            => new Result<T>(default, false, error, ErrorType.UnprocessableEntity);
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