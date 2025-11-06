namespace Body4uHUB.Identity.Application.Commands.EditUser
{
    using FluentValidation;

    using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

    public class EditUserCommandValidator : AbstractValidator<EditUserCommand>
    {
        public EditUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(FirstNameRequired)
                .Length(MinNameLength, MaxNameLength).WithMessage(string.Format(FirstNameLength, MinNameLength, MaxNameLength));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(LastNameRequired)
                .Length(MinNameLength, MaxNameLength).WithMessage(string.Format(LastNameLength, MinNameLength, MaxNameLength));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(PhoneNumberRequired)
                .Matches(PhoneNumberRegex).WithMessage(PhoneNumberInvalid);
        }
    }
}
