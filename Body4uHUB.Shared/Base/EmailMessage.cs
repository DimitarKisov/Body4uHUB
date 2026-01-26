using Body4uHUB.Shared.Domain.Exceptions;
using Body4uHUB.Shared.Domain.Guards;

namespace Body4uHUB.Shared.Domain.Base
{
    public class EmailMessage
    {
        private EmailMessage(string receiver, string subject, string body)
        {
            Receiver = receiver;
            Subject = subject;
            Body = body;
        }

        public static EmailMessage Create(string receiver, string subject, string body)
        {
            Validate(receiver, subject, body);
            return new EmailMessage(receiver, subject, body);
        }

        public string Receiver { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }

        private static void Validate(string receiver, string subject, string body)
        {
            ValidateReceiver(receiver);
            ValidateSubject(subject);
            ValidateBody(body);
        }

        private static void ValidateReceiver(string receiver)
        {
            Guard.AgainstEmptyString<InvalidaSendEmailException>(receiver, nameof(Receiver));
        }

        private static void ValidateSubject(string subject)
        {
            Guard.AgainstEmptyString<InvalidaSendEmailException>(subject, nameof(Subject));
        }

        private static void ValidateBody(string body)
        {
            Guard.AgainstEmptyString<InvalidaSendEmailException>(body, nameof(Body));
        }
    }
}
