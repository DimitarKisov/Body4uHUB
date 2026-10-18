using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;

namespace Body4uHUB.Identity.Application.Commands.EditUser;

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

        // Optional, as on registration: only the format is checked when a number is given.
        RuleFor(x => x.PhoneNumber)
            .Matches(PhoneNumberRegex)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage(PhoneNumberInvalid);
    }
}
