using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(PasswordRequired);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(PasswordRequired)
                .Matches(PasswordRegex).WithMessage(PasswordInvalid);
        }
    }
}
