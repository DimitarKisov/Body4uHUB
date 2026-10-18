using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(EmailRequired)
                .EmailAddress().WithMessage(EmailInvalid);

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(PasswordResetTokenRequired);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(PasswordRequired)
                .Matches(PasswordRegex).WithMessage(PasswordInvalid);
        }
    }
}
