using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application.Events;
using Body4uHUB.Shared.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Body4uHUB.Identity.Infrastructure.Messaging.Consumers
{
    /// <summary>
    /// Sends the email confirmation link. Exceptions are not caught, so a failed send
    /// is retried by MassTransit and ends up in the error queue instead of being lost.
    /// </summary>
    internal sealed class SendEmailConfirmationConsumer(
        IUserRepository userRepository,
        IEmailService emailService,
        IOptions<AppSettings> appSettings,
        ILogger<SendEmailConfirmationConsumer> logger)
        : IConsumer<UserRegisteredEvent>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly ILogger<SendEmailConfirmationConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
        {
            var userId = context.Message.UserId;

            var user = await _userRepository.GetByIdAsync(userId, context.CancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Email confirmation skipped: user {UserId} not found", userId);
                return;
            }

            if (user.IsEmailConfirmed)
            {
                _logger.LogInformation("Email confirmation skipped: user {UserId} is already confirmed", userId);
                return;
            }

            var email = user.ContactInfo.Email;
            var frontendUrl = _appSettings.FrontendUrl.TrimEnd('/');
            var confirmationLink = $"{frontendUrl}/confirm-email?token={Uri.EscapeDataString(user.EmailConfirmationToken)}&email={Uri.EscapeDataString(email)}";

            await _emailService.SendEmailConfirmation(email, $"{user.FirstName} {user.LastName}", confirmationLink);

            _logger.LogInformation("Email confirmation sent to user {UserId}", userId);
        }
    }
}
