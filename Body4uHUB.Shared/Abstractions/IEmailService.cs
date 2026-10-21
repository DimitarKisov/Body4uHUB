using Body4uHUB.Shared.Domain.Enumerations;

namespace Body4uHUB.Shared.Domain.Abstractions
{
    public interface IEmailService
    {
        Task SendEmailConfirmation(string receiverEmail, string firstName, string lastName, string confirmationLink);

        Task SendPasswordReset(string receiverEmail, string firstName, string lastName, string resetLink);
    }
}
