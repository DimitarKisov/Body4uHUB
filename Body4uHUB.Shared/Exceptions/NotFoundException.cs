namespace Body4uHUB.Shared.Exceptions
{
    /// <summary>
    /// Exception when entity is not found
    /// </summary>
    public class NotFoundException : BaseDomainException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with ID '{key}' was not found.")
        {
        }
    }
}
