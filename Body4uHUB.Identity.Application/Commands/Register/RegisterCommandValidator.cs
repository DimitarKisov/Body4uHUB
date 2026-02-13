using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(EmailRequired)
                .EmailAddress().WithMessage(EmailInvalid);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(PasswordRequired)
                .Matches(PasswordRegex).WithMessage(PasswordInvalid);

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(FirstNameRequired)
                .Length(MinNameLength, MaxNameLength).WithMessage(FirstNameLength);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(LastNameRequired)
                .Length(MinNameLength, MaxNameLength).WithMessage(LastNameLength);

            RuleFor(x => x.PhoneNumber)
                .Matches(PhoneNumberRegex)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage(PhoneNumberInvalid);
        }
    }
}