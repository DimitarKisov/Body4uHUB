namespace Body4uHUB.Shared.Exceptions
{
    /// <summary>
    /// Exception for unauthorized access
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException()
            : base("You do not have permission to access this resource.")
        {
        }
    }
}
