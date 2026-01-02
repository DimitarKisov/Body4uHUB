using FluentValidation;

using static Body4uHUB.Identity.Domain.Constants.ModelConstants.UserConstants;
using static Body4uHUB.Shared.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Identity.Application.Commands.CreateTrainer
{
    public class CreateTrainerAccountCommandValidator : AbstractValidator<CreateTrainerAccountCommand>
    {
        public CreateTrainerAccountCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(UserIdRequired);

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage(BioRequired)
                .Length(BioMinLength, BioMaxLength).WithMessage(string.Format(BioLengthMessage, BioMinLength, BioMaxLength));

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(MinYearsOfExperience).WithMessage(string.Format(MinYearsOfExperienceMessage, MinYearsOfExperience));
        }
    }
}
