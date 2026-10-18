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
    /// Sends the password reset link. Exceptions are not caught, so a failed send
    /// is retried by MassTransit and ends up in the error queue instead of being lost.
    /// The token is never logged.
    /// </summary>
    internal sealed class SendPasswordResetConsumer(
        IUserRepository userRepository,
        IEmailService emailService,
        IOptions<AppSettings> appSettings,
        ILogger<SendPasswordResetConsumer> logger)
        : IConsumer<PasswordResetRequestedEvent>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly ILogger<SendPasswordResetConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<PasswordResetRequestedEvent> context)
        {
            var userId = context.Message.UserId;
            var user = await _userRepository.GetByIdAsync(userId, context.CancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Password reset email skipped: user {UserId} not found", userId);
                return;
            }

            var email = user.ContactInfo.Email;
            var frontendUrl = _appSettings.FrontendUrl.TrimEnd('/');
            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(context.Message.Token)}&email={Uri.EscapeDataString(email)}";

            await _emailService.SendPasswordReset(email, $"{user.FirstName} {user.LastName}", resetLink);

            _logger.LogInformation("Password reset email sent to user {UserId}", userId);
        }
    }
}
