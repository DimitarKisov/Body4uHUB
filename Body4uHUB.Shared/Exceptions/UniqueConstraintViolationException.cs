namespace Body4uHUB.Shared.Exceptions
{
    /// <summary>
    /// Exception when saving changes violates a unique index or constraint
    /// </summary>
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(Exception innerException)
            : base("A unique constraint was violated while saving changes.", innerException)
        {
        }
    }
}
