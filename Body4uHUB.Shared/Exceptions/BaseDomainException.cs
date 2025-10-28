namespace Body4uHUB.Shared.Exceptions
{
    public abstract class BaseDomainException : Exception
    {
        private string _error;

        public BaseDomainException()
        {
        }

        public BaseDomainException(string message) : base(message)
        {
            Error = message;
        }

        public string Error
        {
            get => _error ?? base.Message;
            set => _error = value;
        }
    }
}
