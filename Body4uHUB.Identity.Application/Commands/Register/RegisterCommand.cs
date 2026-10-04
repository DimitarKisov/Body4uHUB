using Body4uHUB.Identity.Application.DTOs;
using Body4uHUB.Identity.Application.Mappings;
using Body4uHUB.Identity.Application.Services;
using Body4uHUB.Identity.Application.Settings;
using Body4uHUB.Identity.Domain.Models;
using Body4uHUB.Identity.Domain.Repositories;
using Body4uHUB.Shared.Application;
using Body4uHUB.Shared.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.Common;

namespace Body4uHUB.Identity.Application.Commands.Register
{
    public record RegisterCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PhoneNumber)
        : IRequest<Result<AuthResponseDto>>;

    internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasherService,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            ILogger<RegisterCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        { 
            var userExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (userExists)
            {
                return Result.Conflict<AuthResponseDto>(UserEmailExists);
            }

            var passwordHash = string.Empty;
            try
            {
                passwordHash = _passwordHasherService.HashPassword(request.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password hashing failed for email {Email}", request.Email);
                return Result.InternalServerError<AuthResponseDto>(SomethingWentWrong);
            }

            var emailConfirmationToken = Guid.NewGuid().ToString();

            var user = User.Create(
                passwordHash,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                emailConfirmationToken);

            _userRepository.Add(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var frontendUrl = _appSettings.FrontendUrl.TrimEnd('/');
                var confirmationLink = $"{frontendUrl}/confirm-email?token={emailConfirmationToken}&email={Uri.EscapeDataString(request.Email)}";

                _ = Task.Run(async () =>
                {
                    await _emailService.SendEmailConfirmation(
                            request.Email,
                            request.FirstName + " " + request.LastName,
                            confirmationLink);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email confirmation to {Email}", request.Email);
            }

            var jwtToken = _jwtTokenService.GenerateAccessToken(user.Id, user.ContactInfo.Email, null);

            var response = new AuthResponseDto
            {
                AccessToken = jwtToken,
                User = user.ToDto()
            };

            return Result.Success(response);
        }
    }
}