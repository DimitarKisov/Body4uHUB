using FluentValidation;

using static Body4uHUB.Services.Domain.Constants.ModelConstants.CommonConstants;
using static Body4uHUB.Services.Domain.Constants.ModelConstants.TrainerProfileConstants;

namespace Body4uHUB.Services.Application.Commands.TrainerProfile.Update
{
    public class UpdateTrainerProfileCommandValidator : AbstractValidator<UpdateTrainerProfileCommand>
    {
        public UpdateTrainerProfileCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(TrainerIdRequired);

            RuleFor(x => x.Bio)
                .NotEmpty().WithMessage(BioRequired)
                .Length(BioMinLength, BioMaxLength).WithMessage(string.Format(BioLengthMessage, BioMinLength, BioMaxLength));

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(MinYearsOfExperience).WithMessage(string.Format(MinYearsOfExperienceMessage, MinYearsOfExperience));
        }
    }
}
